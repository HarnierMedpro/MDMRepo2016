using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eMedServiceCorp.Data_Annotations
{
    /*Personalized Data Annotations*/
    [AttributeUsage(AttributeTargets.Property |
     AttributeTargets.Field, AllowMultiple = false)]
    sealed public class CurrentDate : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            bool result = true;
            try
            {
                var date = (DateTime)value;
                if (date > DateTime.Today)
                {
                    result = false;
                }
            }
            catch (Exception)
            {
                result = false;
            }          
            
            return result;
        }
    }
}