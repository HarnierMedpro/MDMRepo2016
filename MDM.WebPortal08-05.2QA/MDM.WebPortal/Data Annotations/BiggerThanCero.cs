using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Data_Annotations
{
    /*Personalized Data Annotations*/
    [AttributeUsage(AttributeTargets.Property |
     AttributeTargets.Field, AllowMultiple = false)]
    sealed public class BiggerThanCero : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            bool result = true;
            try
            {
                var ID = Convert.ToInt32(value);
                if (ID <= 0)
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

    /*Personalized Data Annotations*/
    [AttributeUsage(AttributeTargets.Property |
     AttributeTargets.Field, AllowMultiple = false)]
    sealed public class ValidDate : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            bool result = true;
            try
            {
                var ID = Convert.ToDateTime(value);
                if (ID == new DateTime())
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



    [AttributeUsage(AttributeTargets.Property |
    AttributeTargets.Field, AllowMultiple = false)]
    sealed public class AllowBlankSpace : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            bool result = true;
            try
            {
                var strValue = Convert.ToString(value);
                if (strValue.Contains(" "))
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