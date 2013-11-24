using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Users;

namespace HristoEvtimov.DNN.Modules.FormsQuestionnaireDNN.Components
{
    public class CommonLogic
    {
        public static string PathToTemplates = "~/DesktopModules/FormsQuestionnaireDNN/Templates/";

        public string GetCurrentLocale()
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.Name;
        }

        public string GetUserIPAddress()
        {
            string visitorIPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            
            if (String.IsNullOrEmpty(visitorIPAddress))
                visitorIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrEmpty(visitorIPAddress))
                visitorIPAddress = HttpContext.Current.Request.UserHostAddress;

            return visitorIPAddress;
        }

        //whether an int can be found in a comma separated list of integers
        public bool IntIsInList(int id, string listOfIDs)
        {
            bool result = false;

            string[] arrListOfIDs = listOfIDs.Split(new char[] {','});
            foreach (string idInList in arrListOfIDs)
            {
                int iIDInList = Int32.Parse(idInList);
                if (id == iIDInList)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        //returns true if user is in at least one role from a comma separated list of roles
        public bool UserIsInListOfRoles(UserInfo user, string listOfRoles)
        {
            bool result = false;

            string[] arrlistOfRoles = listOfRoles.Split(new char[] { ',' });
            foreach (string role in arrlistOfRoles)
            {
                if (user.IsInRole(role.Trim()))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
    }
}