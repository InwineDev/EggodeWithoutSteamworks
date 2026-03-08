using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CsModsManager : MonoBehaviour
{
	public static Dictionary<Assembly, CsMod> mods; // Словарь для хранения всех загруженных модов. Ключ — сборка мода, значение — объект CsMod.
	private string modFolderPath; // Путь к папке, где хранятся моды.

	public static List<string> modsForServer = new List<string>();

	[SerializeField]
	private ModListItem modListPrefab; // Префаб элемента списка модов для отображения в панельке модов.
	[SerializeField]
	private Transform modScrollContent; // Объект Content в Scroll View для размещения элементов списка модов.
	[SerializeField]
	private FilterMode iconFilterMode = FilterMode.Bilinear; // Режим фильтрации для иконок модов.
	[SerializeField]
	private string authorPrefix = "Автор"; // Префикс для отображения автора мода.
    private static Dictionary<string, string> assemblyToDirectory = new Dictionary<string, string>();

    private void Awake()
    {
        // Подписываемся на событие разрешения сборок для поддержки библиотек
        AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

        print("1");
        if (mods != null)
        {
            print("2");
            foreach (CsMod mod in mods.Values)
            {
                print("3");
                ModConfig config = mod.config;
                string iconPath = Path.Combine(mod.modDir, config.iconPath);
                SpawnModListItem(config, iconPath);
            }
            return;
        }

        try
        {
            LoadMods();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }

        mods = mods;
    }

    // Обработчик для автоматической загрузки зависимостей (библиотек)
    private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
    {
        var assemblyName = new AssemblyName(args.Name).Name;

        if (args.RequestingAssembly != null && assemblyToDirectory.TryGetValue(args.RequestingAssembly.FullName, out var modDir))
        {
            // Ищем в корневой папке мода
            var dllPath = Path.Combine(modDir, assemblyName + ".dll");
            if (File.Exists(dllPath))
            {
                return Assembly.LoadFrom(dllPath);
            }

            // Ищем в подпапке libs
            var libsPath = Path.Combine(modDir, "libs", assemblyName + ".dll");
            if (File.Exists(libsPath))
            {
                return Assembly.LoadFrom(libsPath);
            }
        }

        return null;
    }

    public void LoadMods()
    {
        print("4");
        mods = new Dictionary<Assembly, CsMod>();
        modFolderPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "mods");

        if (!Directory.Exists(modFolderPath))
        {
            Directory.CreateDirectory(modFolderPath);
            Debug.Log($"Папка модов создана: {modFolderPath}");
            return;
        }

        foreach (string modDir in Directory.GetDirectories(modFolderPath))
        {
            string configPath = Path.Combine(modDir, "config.json");

            if (!File.Exists(configPath))
            {
                Debug.LogWarning($"Пропущен мод в {modDir}: отсутствует config.json");
                continue;
            }

            try
            {
                Assembly asm = null;

                string[] csFiles = Directory.GetFiles(modDir, "*.cs", SearchOption.TopDirectoryOnly);

                if (csFiles.Length > 0)
                {
                    Debug.Log($"Найдены .cs файлы в {modDir}, компилируем...");
                    asm = CompileCSharpFiles(csFiles, modDir);

                    if (asm == null)
                    {
                        Debug.LogError($"Не удалось скомпилировать мод из {modDir}");
                        continue;
                    }
                }
                else
                {
                    string[] dllFiles = Directory.GetFiles(modDir, "*.dll", SearchOption.TopDirectoryOnly);

                    if (dllFiles.Length == 0)
                    {
                        Debug.LogWarning($"Пропущен мод в {modDir}: не найдено ни .cs, ни .dll файлов");
                        continue;
                    }

                    string mainDllPath = dllFiles[0];
                    asm = Assembly.Load(File.ReadAllBytes(mainDllPath));
                }

                if (mods.ContainsKey(asm))
                {
                    Debug.LogWarning($"Мод из {modDir} уже загружен");
                    continue;
                }

                assemblyToDirectory[asm.FullName] = modDir;

                CsMod mod = new CsMod(asm, modDir);
                string configJson = File.ReadAllText(configPath);
                mod.config = JsonUtility.FromJson<ModConfig>(configJson);

                mods.Add(asm, mod);
                mod.Activate();

                string iconPath = Path.Combine(modDir, mod.config.iconPath);
                SpawnModListItem(mod.config, iconPath);

                Debug.Log($"Мод загружен: {mod.config.name}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка при загрузке мода из {modDir}: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
    private Assembly CompileCSharpFiles(string[] csFiles, string modDir)
    {
        try
        {
            var syntaxTrees = csFiles.Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file))).ToList();

            var references = new List<MetadataReference>();
            var loadedAssemblyLocations = new HashSet<string>();

            // 1. Собираем все сборки, загруженные в Unity
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (!assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location))
                    {
                        if (loadedAssemblyLocations.Add(assembly.Location)) // Проверка на дубликаты
                        {
                            references.Add(MetadataReference.CreateFromFile(assembly.Location));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Не удалось добавить ссылку на сборку {assembly.FullName}: {ex.Message}");
                }
            }

            // 2. Дополнительно добавляем библиотеки из папки мода (libs)
            string libsPath = Path.Combine(modDir, "libs");
            if (Directory.Exists(libsPath))
            {
                foreach (var dllPath in Directory.GetFiles(libsPath, "*.dll"))
                {
                    try
                    {
                        if (loadedAssemblyLocations.Add(dllPath)) // Проверка на дубликаты
                        {
                            references.Add(MetadataReference.CreateFromFile(dllPath));
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Не удалось добавить ссылку на библиотеку мода {dllPath}: {ex.Message}");
                    }
                }
            }

            string assemblyName = Path.GetFileNameWithoutExtension(modDir) + ".Mod"; // Уникальное имя сборки

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: syntaxTrees,
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    Debug.LogError($"Ошибка компиляции мода {assemblyName}:");
                    foreach (var diagnostic in result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
                    {
                        Debug.LogError($"  {diagnostic.GetMessage()} (в {diagnostic.Location.SourceTree?.FilePath})");
                    }
                    return null;
                }

                Debug.Log($"Мод {assemblyName} успешно скомпилирован из .cs файлов.");
                ms.Seek(0, SeekOrigin.Begin);
                return Assembly.Load(ms.ToArray());
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Критическая ошибка при компиляции мода из папки {modDir}: {ex.Message}\n{ex.StackTrace}");
            return null;
        }
    }

    // Создает элемент списка модов в панельке модов.
    public void SpawnModListItem(ModConfig config, string iconPath)
	{
		// Создаем объект элемента списка из префаба.
		GameObject listObj = GameObject.Instantiate(modListPrefab.gameObject, modScrollContent);

		// Получаем компонент ModListItem для настройки.
		ModListItem listItem = listObj.GetComponent<ModListItem>();

		// Загружаем иконку мода и настраиваем элемент списка.
		listItem.icon.sprite = CsModAPI.LoadSpriteFull(iconPath, iconFilterMode);
		listItem.modName.text = config.name;
		listItem.description.text = config.description;
		listItem.author.text = $"{authorPrefix} {config.author}";
	}

	private void Start()
	{
		// Активируем все моды после загрузки сцены.
		foreach (CsMod mod in mods.Values)
		{
			mod.Activate();
		}
	}
}