using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Socialize.Logic
{
    public class OptionalMatchContainer
    {
        //singlton implementation
        private static OptionalMatchContainer OptionalContainerInstance;

        public static OptionalMatchContainer GetOptionalMatchContainerInstance()
        {
            if (OptionalContainerInstance == null)
            {
                OptionalContainerInstance = new OptionalMatchContainer();
            }
            return OptionalContainerInstance;
        }

        private OptionalMatchContainer()
        {

        }

    }
}