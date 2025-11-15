namespace BloomEngine.Utilities;

public static class TypeHelper
{
    public static bool IsNumericType(Type type) => Type.GetTypeCode(type) switch
    {
        TypeCode.Byte => true,
        TypeCode.SByte => true,
        TypeCode.UInt16 => true,
        TypeCode.UInt32 => true,
        TypeCode.UInt64 => true,
        TypeCode.Int16 => true,
        TypeCode.Int32 => true,
        TypeCode.Int64 => true,
        TypeCode.Decimal => true,
        TypeCode.Double => true,
        TypeCode.Single => true,
        _ => false
    };

    public static bool IsFloatType(Type type) => type == typeof(float) || type == typeof(double) || type == typeof(decimal);

    public static bool IsSignedType(Type type) => type != typeof(uint) && type != typeof(ulong) && type != typeof(ushort) && type != typeof(byte);
}