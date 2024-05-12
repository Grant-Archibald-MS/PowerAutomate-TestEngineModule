using System;

namespace testengine.module.powerautomate.tests
{
    public class ValueContainerConvertorTest
    {
        public static TheoryData<string, string, string, object> Cases =
            new()
            {
                { "{'a':1}","a", "System.Int32", 1 },
                { "{'a':1.5}","a", "System.Double", 1.5 },
                { "{'a':'a'}", "a", "System.String", "a" },
                { "{'parent':{'child':'a'}}", "parent/child", "System.String", "a" },
                { "{'value':true}", "value", "System.Boolean", true },
                { "{'value':false}", "value", "System.Boolean", false },
                { "{'when':'=Date(2024,12,31)'}", "when", "System.DateTime", new DateTime(2024, 12, 31) },
                { "{'id':'=GUID(''00000000-0000-0000-0000-000000000000'')'}", "id", "System.Guid", Guid.Empty },
                { "{'value':'=a+b','a':1,'b':2}", "value", "System.Int32", 3 }
            };

        [Theory, MemberData(nameof(Cases))]
        public void Convert(string json, string name, string expectedType, object value)
        {
            // Arrange
            var convertor = new ValueContainerConvertor();

            // Act
            var result = convertor.Convert(json);

            // Assert
            var first = result.Where(x => x.Key == name).First();
            Assert.Equal(name, first.Key);

            var method = first.Value.GetType().GetMethod("GetValue", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var genericMethod = method?.MakeGenericMethod(Type.GetType(expectedType));

            Assert.Equal(value, genericMethod.Invoke(first.Value, null));
        } 
    }
}
