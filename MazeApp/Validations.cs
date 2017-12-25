using MazeApp.Exceptions;
using System.Collections.Generic;
using System.Reflection;

namespace MazeApp
{
    public static class Validations
    {

        /// <summary>
        /// Validates App Configuration parameters 
        /// </summary>
        /// <param name="settings"></param>
        public static void ValidateConfiguration(MazeSettings settings)
        {
            var properties = settings.GetType().GetTypeInfo().DeclaredProperties;

            // check for null , 0 or negative int and throw on invalid
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(string))
                {
                    string val = (string)property.GetValue(settings);

                    if (string.IsNullOrEmpty(val))
                        throw new InvalidConfigurationParameterException(property.Name, "Parameter can not be Null or Empty");

                }
                else if (property.PropertyType == typeof(int))
                {
                    int? val = (int)property.GetValue(settings);

                    // we require a minimum of 2x2
                    if ((val ?? 0) <= 1)
                        throw new InvalidConfigurationParameterException(property.Name, "Parameter can not be Null or smalle than 2");

                }

            }

            // nice, now lets check the characters
            var validList = new List<string>();

            if (settings.OpenChar.Length > 1)
                throw new InvalidConfigurationParameterException("OpenChar", "Parameter is longer than one character");

            validList.Add(settings.OpenChar);

            if (settings.WallChar.Length > 1 || validList.Contains(settings.WallChar))
                throw new InvalidConfigurationParameterException("WallChar", "Parameter is longer than one character or already in use");

            validList.Add(settings.WallChar);


            if (settings.StartChar.Length > 1 || validList.Contains(settings.StartChar))
                throw new InvalidConfigurationParameterException("StartChar", "Parameter is longer than one character or already in use");

            validList.Add(settings.StartChar);

            if (settings.FinishChar.Length > 1 || validList.Contains(settings.FinishChar))
                throw new InvalidConfigurationParameterException("FinishChar", "Parameter is longer than one character or already in use");

            // clean up 
            validList.Clear();

        }


       

    }
}
