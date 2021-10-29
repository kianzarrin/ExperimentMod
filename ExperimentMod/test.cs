using System;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using KianCommons;

class Test {
    const string FILE = "dummy.d";
    public static void Run() {
        //var foo = Foo.Create();
        //Serialize(foo);
        var foo2 = Deserialize();
        foo2.Prints();
    }

    public static void Serialize(Foo foo) => File.WriteAllBytes(FILE, Util.Serialize(foo));
    public static Foo Deserialize() => Util.Deserialize(File.ReadAllBytes(FILE)) as Foo;
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


public static class Util {
    static BinaryFormatter GetBinaryFormatter =>
    new BinaryFormatter { AssemblyFormat = FormatterAssemblyStyle.Simple };

    public static object Deserialize(byte[] data) {
        if (data == null || data.Length == 0) return null;

        var memoryStream = new MemoryStream();
        memoryStream.Write(data, 0, data.Length);
        memoryStream.Position = 0;
        return GetBinaryFormatter.Deserialize(memoryStream);
    }

    public static byte[] Serialize(object obj) {
        if (obj == null) return null;
        var memoryStream = new MemoryStream();
        GetBinaryFormatter.Serialize(memoryStream, obj);
        memoryStream.Position = 0; // redundant
        return memoryStream.ToArray();
    }
}

