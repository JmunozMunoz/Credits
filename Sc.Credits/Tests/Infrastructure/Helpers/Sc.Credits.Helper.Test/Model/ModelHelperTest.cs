using RandomTestValues;
using System.Reflection;

namespace Sc.Credits.Helper.Test.Model
{
    public static class ModelHelperTest
    {
        public static bool TestModel<T>()
          where T : new()
        {
            return ValidateModel(InstanceModel<T>());
        }

        public static bool ValidateModel<T>(T testModel)
        {
            if (testModel == null)
            {
                return false;
            }

            PropertyInfo[] properties = testModel.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.GetValue(testModel) == null)
                {
                    return false;
                }
            }

            return true;
        }

        public static T InstanceModel<T>()
             where T : new()
        {
            return RandomValue.Object<T>();
        }
    }
}