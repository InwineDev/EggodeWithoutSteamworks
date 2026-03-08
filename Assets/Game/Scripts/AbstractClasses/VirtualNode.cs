using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using static scriptor;
using static UnityEngine.Rendering.DebugUI;

public abstract class VirtualNode
{
    // Базовые свойства
    public string Id { get; set; }
    public List<string> SocketsFromOutputIds { get; set; } = new List<string>();
    public List<string> SocketsFromInputIds { get; set; } = new List<string>();
    public List<string> OutputSocketIds { get; set; } = new List<string>();
    public List<string> InputSocketIds { get; set; } = new List<string>();
    public bool ExecuteOnStart { get; set; }
    public int ExecutionPriority { get; set; }
    // Основной метод выполнения
    public abstract void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered);
    protected VirtualGraphExecutor Executor { get; private set; }

    public void SetExecutor(VirtualGraphExecutor executor)
    {
        Executor = executor;
    }
    // Вспомогательные методы
    protected T GetInputValue<T>(Dictionary<string, object> context, string socketId, T defaultValue = default)
    {
        if (context.TryGetValue(socketId, out var value))
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка преобразования значения для сокета {socketId}: {ex.Message}");
            }
        }
        return defaultValue;
    }

    protected void SetOutputValue(Dictionary<string, object> context, string socketId, object value)
    {
        context[socketId] = value;
        Debug.Log(context[socketId]);
    }

    protected virtual void ProcessInputs(Dictionary<string, object> context)
    {
        // Базовая реализация обработки входов (можно переопределить)
        foreach (var inputSocketId in InputSocketIds)
        {
            if (!context.ContainsKey(inputSocketId))
            {
                Debug.LogWarning($"Не получено значение для входа {inputSocketId} в ноде {GetType().Name}");
            }
        }
    }
}
// Стартовая нода
public class VirtualOnStartNode : VirtualNode
{
    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        Debug.Log($"Старт выполнения графа! Вызвано объектом: {caller.name}");
        Executor?.ExecuteConnections(this, context, caller, triggered);
        // Пример взаимодействия с вызывающим объектом
        /*        if (caller is GameObject gameObject)
                {
                    NetworkServer.Destroy(gameObject);
                }*/
    }
}
public class VirtualOnTriggerEnterNode : VirtualNode
{
    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        Debug.Log($"Старт выполнения графа через trigger! Вызвано объектом: {caller.name}");
        Executor?.ExecuteConnections(this, context, caller, triggered);
        // Пример взаимодействия с вызывающим объектом
        /*        if (caller is GameObject gameObject)
                {
                    NetworkServer.Destroy(gameObject);
                }*/
    }
}

public class VirtualOnCollisionEnterNode : VirtualNode
{
    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        Debug.Log($"Старт выполнения графа через collision! Вызвано объектом: {caller.name}");
        Executor?.ExecuteConnections(this, context, caller, triggered);
    }
}
public class VirtualDestroyNode : VirtualNode
{
    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        Debug.Log($"Попытка поедания объекта");
        foreach (var socketsId in SocketsFromInputIds)
        {
            if (context.TryGetValue(socketsId, out var obj) && obj is List<GameObject> gameObj)
            {
                foreach (var item1 in gameObj)
                {
                    NetworkServer.Destroy(item1);
                    Debug.Log($"Объект уничтожен: {item1.name}");
                }
                Executor?.ExecuteConnections(this, context, caller, triggered);
            }
        }
    }
}
public class VirtualChangeGravityNode : VirtualNode
{
    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        Debug.Log($"Попытка изменения гравитации объекта");
        GameObject gObj = null;
        bool gBool = false;
        foreach (var socketsId in SocketsFromInputIds)
        {
            if (context.TryGetValue(socketsId, out var obj) && obj is GameObject gameObj)
            {
                gObj = gameObj;
                Debug.Log(gObj);
            }
            if (context.TryGetValue(socketsId, out var booling) && booling is bool booled)
            {
                gBool = booled;
                Debug.Log(gBool);
            }
        }
        try
        {
            gObj.GetComponent<Rigidbody>().isKinematic = gBool;
            Debug.Log($"Объект isKinematic {gBool} на обжект: {gObj.name}");
            Executor?.ExecuteConnections(this, context, caller, triggered);
        }
        catch
        {
            Debug.Log("Error");
            //Executor?.ExecuteConnections(this, context, caller);
        }
    }
}

public class VirtualDamageNode : VirtualNode
{
    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        Debug.Log($"Попытка урона объекта");
        foreach (var socketsId in SocketsFromInputIds)
        {
            if (context.TryGetValue(socketsId, out var obj) && obj is int uron)
            {
                try
                {
                    HealthPlayer healthPlayer = triggered.GetComponent<HealthPlayer>();
                    bool uron2 = serverProperties.instance.hp;
                    if (uron2 & healthPlayer != null)
                    {
                        healthPlayer.health -= uron;
                        if (healthPlayer.health <= 0)
                        {
                            healthPlayer.health = 100;
                            healthPlayer.hp.text = $"{healthPlayer.health} HP";
                        }
                        Debug.Log($"Объект зауронен: {healthPlayer}");
                        Executor?.ExecuteConnections(this, context, caller, triggered);
                    }
                } 
                catch
                {

                }
            }
        }
    }
}
public class VirtualShowMessageNode : VirtualNode
{
    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        Debug.Log($"Попытка показать месседж объекта");
        string txt = "null";
        float time = 0f;
        foreach (var socketsId in SocketsFromInputIds)
        {
            if (context.TryGetValue(socketsId, out var fl) && fl is float ti)
            {
                time = ti;
            }
            if (context.TryGetValue(socketsId, out var str) && str is string text)
            {
                txt = text;
            }
        }
        try
        {
            Debug.Log($"Показываю месседж шока {txt} {time}");
            triggered.GetComponent<userSettingNotCam>().messageController.ShowMessage(txt, time);
        }
        catch
        {

        }
    }
}
public class VirtualTpNode : VirtualNode
{
    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        Debug.Log($"Попытка телепортации объекта");
/*        GameObject objectToTp = null;
        foreach (var socketsId in SocketsFromInputIds)
        {
            if (context.TryGetValue(socketsId, out var fl) && fl is GameObject ti)
            {
                objectToTp = ti;
            }
        }*/
        foreach (var socketsId in SocketsFromInputIds)
        {
            if (context.TryGetValue(socketsId, out var obj) && obj is Vector3 position)
            {
                try
                {
                    triggered.transform.position = position;
                    Debug.Log($"Объект телепортирован!");
                }
                catch (Exception ex)
                {
                    Debug.Log($"Объект НЕ телепортирован! " + ex);
                }
            }
        }
    }
}
public class VirtualFloatNode : VirtualNode
{
    public float Value { get; set; }

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        foreach (var socketsId in SocketsFromOutputIds)
        {
            SetOutputValue(context, socketsId, Value);
            Debug.Log($"FloatNode передал значение {Value} в сокет {socketsId}");
        }
    }
}

public class VirtualVectorNode : VirtualNode
{
    private float x;
    private float y;
    private float z;

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        Debug.Log($"EXECUTED MEMES");
        try
        {
            if (context.TryGetValue(SocketsFromInputIds[0], out var fl) && fl is float ti)
            {
                x = ti;
            }
            if (context.TryGetValue(SocketsFromInputIds[1], out var fl1) && fl is float ti1)
            {
                y = ti1;
            }
            if (context.TryGetValue(SocketsFromInputIds[2], out var fl2) && fl is float ti2)
            {
                z = ti2;
            }
        }
        catch
        {
            Debug.Log($"OhFUCK");
        }

        Vector3 vector = new Vector3(x, y, z);
        foreach (var socketsId in SocketsFromOutputIds)
        {
            SetOutputValue(context, socketsId, vector);
            Debug.Log($"VectorNode передал значение {vector} в сокет {socketsId}");
        }
    }
}
public class VirtualBoolNode : VirtualNode
{
    public bool Value { get; set; }

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        foreach (var socketsId in SocketsFromOutputIds)
        {
            SetOutputValue(context, socketsId, Value);
            Debug.Log($"BoolNode передал значение {Value} в сокет {socketsId}");
        }
    }
}

public class VirtualBooledNode : VirtualNode
{
    public int OpType { get; set; }

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        object x = null;
        object y = null;
        bool result = false;

        try
        {
            if (SocketsFromInputIds.Count > 0 && context.TryGetValue(SocketsFromInputIds[0], out var val1))
            {
                x = val1;
            }
            if (SocketsFromInputIds.Count > 1 && context.TryGetValue(SocketsFromInputIds[1], out var val2))
            {
                y = val2;
            }

            result = PerformOperation(x, y, OpType);
        }
        catch (Exception ex)
        {
            Debug.Log($"Ошибка в VirtualBooledNode: {ex.Message}");
            result = false;
        }

        foreach (var socketsId in SocketsFromOutputIds)
        {
            SetOutputValue(context, socketsId, result);
            Debug.Log($"BooledNode передал значение {result} в сокет {socketsId}");
        }
    }

    private bool PerformOperation(object x, object y, int opType)
    {
        if (x is string strX && y is string strY)
        {
            return PerformStringOperation(strX, strY, opType);
        }
        else if (IsNumber(x) && IsNumber(y))
        {
            return PerformNumberOperation(x, y, opType);
        }
        else
        {
            return PerformUniversalOperation(x, y, opType);
        }
    }

    private bool PerformStringOperation(string x, string y, int opType)
    {
        switch (opType)
        {
            case 0: // ==
                return string.Equals(x, y, StringComparison.Ordinal);

            case 1: // >
                return string.Compare(x, y, StringComparison.Ordinal) > 0;

            case 2: // <
                return string.Compare(x, y, StringComparison.Ordinal) < 0;

            case 3: // !=
                return !string.Equals(x, y, StringComparison.Ordinal);

            default:
                Debug.LogWarning($"Неизвестный тип операции для строк: {opType}");
                return false;
        }
    }

    private bool PerformNumberOperation(object x, object y, int opType)
    {
        double numX = Convert.ToDouble(x);
        double numY = Convert.ToDouble(y);

        switch (opType)
        {
            case 0: // ==
                return Math.Abs(numX - numY) < 0.000001;

            case 1: // >
                Debug.Log(numX > numY + numX + numY);
                return numX > numY;

            case 2: // <
                Debug.Log(numX < numY + numX + numY);
                return numX < numY;

            case 3: // !=
                return Math.Abs(numX - numY) > 0.000001;

            default:
                Debug.LogWarning($"Неизвестный тип операции для чисел: {opType}");
                return false;
        }
    }

    private bool PerformUniversalOperation(object x, object y, int opType)
    {
        switch (opType)
        {
            case 0:
                return object.Equals(x, y);

            case 1:
                try
                {
                    double numX = Convert.ToDouble(x);
                    double numY = Convert.ToDouble(y);
                    return numX > numY;
                }
                catch
                {
                    Debug.LogWarning("Невозможно сравнить объекты как числа для операции >");
                    return false;
                }

            case 2:
                try
                {
                    double numX = Convert.ToDouble(x);
                    double numY = Convert.ToDouble(y);
                    return numX < numY;
                }
                catch
                {
                    Debug.LogWarning("Невозможно сравнить объекты как числа для операции <");
                    return false;
                }

            case 3:
                return !object.Equals(x, y);

            default:
                Debug.LogWarning($"Неизвестный тип операции: {opType}");
                return false;
        }
    }

    private bool IsNumber(object value)
    {
        return value is int || value is float || value is double || value is decimal ||
               value is long || value is short || value is byte;
    }
}

public class VirtualIntNode : VirtualNode
{
    public int Value { get; set; }

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        foreach (var socketsId in SocketsFromOutputIds)
        {
            SetOutputValue(context, socketsId, Value);
            Debug.Log($"IntNode передал значение {Value} в сокет {socketsId}");
        }
    }
}

public class VirtualStringNode : VirtualNode
{
    public string Value { get; set; }

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        foreach (var socketsId in SocketsFromOutputIds)
        {
            SetOutputValue(context, socketsId, Value);
            Debug.Log($"StringNode передал значение {Value} в сокет {socketsId}");
        }
    }
}
public class VirtualGetValueNode : VirtualNode
{
    public string Value { get; set; }

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        try
        {
            List<GameObject> obj = null;

            foreach (var socketsId in SocketsFromInputIds)
            {
                Debug.Log(socketsId);
                if (context.TryGetValue(socketsId, out var fl) && fl is List<GameObject> ti)
                {
                    obj = ti;
                }
            }

            var outVar = obj[0].GetComponent<VarDictionary>().values[Value];

            foreach (var socketsId in SocketsFromOutputIds)
            {
                SetOutputValue(context, socketsId, outVar);
                Debug.Log($"GetValue передал значение {outVar} в сокет {socketsId}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"GetValue взорвался при исполнении {e}");
        }
    }
}
public class VirtualSetValueNode : VirtualNode
{
    public string Value { get; set; }

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        try
        {
            //var outVar = triggered.GetComponent<VarDictionary>().values[Value];
            List<GameObject> obj = null;
            object newVar = null;
            foreach (var socketsId in SocketsFromInputIds)
            {
                if (context.TryGetValue(socketsId, out var fl) && fl is List<GameObject> ti)
                {
                    obj = ti;
                }
                if (context.TryGetValue(socketsId, out var str) && str is var outVar)
                {
                    newVar = outVar;
                }
            }
            foreach (var item1 in obj)
            {
                Debug.Log("SET VALUE " + newVar + " with key " + Value + item1);
                item1.GetComponent<VarDictionary>().values[Value] = newVar;
                Debug.Log("set val " + item1.name);
            }
        }
        catch
        {

        }
    }
}

public class VirtualIntRandomNode : VirtualNode
{
    public int ValueFrom { get; set; }
    public int ValueTo { get; set; }

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        foreach (var socketsId in SocketsFromOutputIds)
        {
            int newValue = UnityEngine.Random.Range(ValueFrom, ValueTo);
            SetOutputValue(context, socketsId, newValue);
            Debug.Log($"IntRandomNode передал значение {newValue} в сокет {socketsId}");
        }
    }
}

public class VirtualFloatRandomNode : VirtualNode
{
    public float ValueFrom { get; set; }
    public float ValueTo { get; set; }

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        foreach (var socketsId in SocketsFromOutputIds)
        {
            float newValue = UnityEngine.Random.Range(ValueFrom, ValueTo);
            SetOutputValue(context, socketsId, newValue);
            Debug.Log($"FloatRandomNode передал значение {newValue} в сокет {socketsId}");
        }
    }
}
public class VirtualThisObjectNode : VirtualNode
{
    public float Value { get; set; }

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        GameObject gameObjectCaller = caller as GameObject;
        List<GameObject> list = new List<GameObject>();
        list.Add(gameObjectCaller);
        if (gameObjectCaller == null)
        {
            Debug.LogError("VirtualThisObjectNode: caller is not a GameObject");
            return;
        }

        foreach (var outputSocketId in SocketsFromOutputIds)
        {
            context[outputSocketId] = list;
            Debug.Log($"VirtualThisObjectNode записал значение {list} в сокет {outputSocketId}");
        }
    }
}
public class VirtualTimeSleepNode : VirtualNode
{
    public float DelaySeconds { get; set; } = 0f;

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        foreach (var socketsId in SocketsFromInputIds)
        {
            if (context.TryGetValue(socketsId, out var obj) && obj is float floatValue)
            {
                try { DelaySeconds = Convert.ToSingle(floatValue); }
                catch { Debug.Log($"Ошибка задержки: {floatValue}"); }
            }
        }

        Debug.Log($"Старт задержки: {DelaySeconds}");
        ModLoader.instance.StartCoroutine(DelayCoroutine(context, caller, triggered));

        // Не вызываем ExecuteConnections здесь, так как это сделает корутина
    }

    private IEnumerator DelayCoroutine(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        yield return new WaitForSeconds(DelaySeconds);
        Debug.Log($"TimeSleepNode закончил задержку");
        /*        foreach (var outputSocketId in OutputSocketIds)
                {
                    context[outputSocketId] = DelaySeconds;
                    Debug.Log($"TimeSleepNode записал значение {DelaySeconds} в сокет {outputSocketId}");
                }*/

        Executor?.ExecuteConnections(this, context, caller, triggered);
    }
}

public class VirtualIfNode : VirtualNode
{
    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        bool ifval = false;
        foreach (var socketsId in SocketsFromInputIds)
        {
            if (context.TryGetValue(socketsId, out var fl) && fl is bool ti)
            {
                ifval = ti;
            }
        }
        Debug.Log(ifval);
        if (ifval)
        {
            Executor?.ExecuteConnections(this, context, caller, triggered, 0);
        }
        else
        {
            Executor?.ExecuteConnections(this, context, caller, triggered, 1);
        }
    }

}
public class VirtualForNode : VirtualNode
{
    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        int howMany = 0;

        foreach (var socketsId in SocketsFromInputIds)
        {
            if (context.TryGetValue(socketsId, out var fl) && fl is int ti)
            {
                howMany = ti;
            }
        }

        for (int i = 0; i < howMany; i++)
        {
            Executor?.ExecuteConnections(this, context, caller, triggered, 0);
        }
            
        Executor?.ExecuteConnections(this, context, caller, triggered, 1);
    }

}

public class VirtualMathNode : VirtualNode
{
    public enum Operation { Add, Subtract, Multiply, Divide }
    public Operation OpType;

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        if (context.TryGetValue("inputA", out var aObj) &&
            context.TryGetValue("inputB", out var bObj))
        {
            float a = (float)aObj;
            float b = (float)bObj;
            float result = OpType switch
            {
                Operation.Add => a + b,
                Operation.Subtract => a - b,
                Operation.Multiply => a * b,
                Operation.Divide => a / b,
                _ => 0
            };

            Debug.Log($"Результат: {result} (вызвано {caller.name})");
            context["output"] = result;

 /*           // Пример применения результата к вызывающему объекту
            if (caller is MonoBehaviour behaviour)
            {
                behaviour.transform.localScale *= result * 0.1f;
            }*/
        }
    }
}


public class VirtualObjectFromIdNode : VirtualNode
{
    public string Value { get; set; }
    public name24 fromId { get; set; }

    public override void Execute(Dictionary<string, object> context, GameObject caller, GameObject triggered)
    {
        List<GameObject> fromIds = new List<GameObject>();
        foreach (var item1 in serverProperties.instance.allBlocks)
        {
            if(item1.id == Value)
            {
                fromId = item1;
                fromIds.Add(item1.gameObject);
            }
        }
        if (fromId == null)
        {
            Debug.LogError("VirtualObjectFromIdNode: caller is not a GameObject");
            return;
        }

        foreach (var outputSocketId in SocketsFromOutputIds)
        {
            context[outputSocketId] = fromIds;
            Debug.Log($"VirtualObjectFromIdNode записал значение {fromIds} в сокет {outputSocketId}");
        }
    }
}