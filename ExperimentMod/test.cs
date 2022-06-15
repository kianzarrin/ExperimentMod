using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using KianCommons;

class Test {
    const string FILE = "dummy.d"; 
    public static void Run() {
        // System.Runtime.Serialization.Formatters.Binary.ObjectReader.ReadValue
        // ReadType
        { 
            var bar = Bar.Create();
            //Serialize(bar); 
        }
        {
            var bar = Deserialize();
            bar.Prints();
        } 
    }

    public static void Serialize(Bar bar) => File.WriteAllBytes(FILE, Util.Serialize(bar));
    public static Bar Deserialize() => Util.Deserialize(File.ReadAllBytes(FILE)) as Bar;
}

[Serializable]
public class Foo {
    public int X1;

    public int X2; // drop

    public int X3; // add

    //public static Foo Create() => new Foo { X1 = 1, X2 = 2 };
    public override string ToString() => $"Foo{{X1={X1}, x2={X2}, x3={X3}}}";
    public void Prints() => Log.Debug(ToString());
}

[Serializable]

public class Bar {
    public List<Foo> Foos;
    public Bar Init() {
        Foos = new();
        Foos.Add(new Foo { X1 = 1, X2 = 2, X3 = 3 });
        Foos.Add(new Foo { X1 = 11, X2 = 22, X3 = 33 });
        return this;
    }
    public static Bar Create() => new Bar().Init();
    public override string ToString() => $"Bar[{Foos.ToSTR()}]";
    public void Prints() => Log.Debug(ToString());

}

sealed class MyBinder : SerializationBinder {
    public override Type BindToType(string assemblyName, string typeName) {
        Log.Called(assemblyName, typeName);
        return null;
    }
}

public static class Util {
    static BinaryFormatter GetBinaryFormatter =>
        new BinaryFormatter { AssemblyFormat = FormatterAssemblyStyle.Simple };
    static BinaryFormatter GetFormatter() {
        var ret = GetBinaryFormatter;
        ret.Binder = new MyBinder();
        return ret;
    }

    public static object Deserialize(byte[] data) {
        if (data == null || data.Length == 0) return null;

        var memoryStream = new MemoryStream();
        memoryStream.Write(data, 0, data.Length);
        memoryStream.Position = 0;
        return GetFormatter().Deserialize(memoryStream);
    }

    public static byte[] Serialize(object obj) {
        if (obj == null) return null;
        var memoryStream = new MemoryStream();
        GetBinaryFormatter.Serialize(memoryStream, obj);
        memoryStream.Position = 0; // redundant
        return memoryStream.ToArray();
    }
}

