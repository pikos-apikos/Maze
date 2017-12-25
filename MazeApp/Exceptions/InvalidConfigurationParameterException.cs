using System;
using System.Collections.Generic;
using System.Text;

namespace MazeApp.Exceptions
{
    public class InvalidConfigurationParameterException : Exception
    {
        public InvalidConfigurationParameterException()
        {

        }

        /// <summary>
        /// Invalid Configuration Parameter Exception
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="uxPhrase">Useful message</param>
        public InvalidConfigurationParameterException(string parameterName, string uxPhrase) 
            : base(String.Format("Configuration Parameter {0} is invalid, {1}", parameterName, uxPhrase))
        {

        }

    }
}
