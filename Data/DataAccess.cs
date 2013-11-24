/*
 * PackFlash http://www.packflash.com
 * Copyright (c) 2012 Hristo Evtimov
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
 * documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
 * to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions 
 * of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 *
 */
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System;
using DotNetNuke.Common.Utilities;

namespace HristoEvtimov.DNN.Modules.FormsQuestionnaireDNN.Data
{
    public class DataAccess
    {
        private FQDNNEntities GetContext()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString;
            FQDNNEntities context = new FQDNNEntities("metadata=res://*/Data.Model1.csdl|res://*/Data.Model1.ssdl|res://*/Data.Model1.msl;provider=System.Data.SqlClient;provider connection string=\"" + connectionString + ";MultipleActiveResultSets=True\"");

            return context;
        }

        //get list of all forms
        public List<FQDNN_Form> GetForms()
        {
            FQDNNEntities context = GetContext();

            var Forms = from c in context.FQDNN_Form
                        select c;

            return Forms.ToList();
        }

        //get all the active forms for the current portal/language combination
        public List<FQDNN_Form> GetActiveForms(string Locale, int PortalID)
        {
            FQDNNEntities context = GetContext();

            var Forms = from c in context.FQDNN_Form
                        where c.Active == true && 
                            c.ActiveFrom <= DateTime.Now && 
                            c.ActiveTo >= DateTime.Now && 
                            c.Deleted == false &&
                            c.Locale == Locale &&
                            c.PortalID == PortalID
                        select c;

            return Forms.ToList();
        }

        //get the active form for the current portal/language combination
        public FQDNN_Form GetActiveForm(string formGUID, string locale, int portalID, string portalDefaultLocale)
        {
            FQDNNEntities context = GetContext();

            var Forms = from c in context.FQDNN_Form
                        where c.FormGUID == new Guid(formGUID) && 
                            c.Active == true &&
                            c.ActiveFrom <= DateTime.Now &&
                            c.ActiveTo >= DateTime.Now &&
                            c.Deleted == false &&
                            c.PortalID == portalID
                        select c;

            List<FQDNN_Form> forms = Forms.ToList();

            FQDNN_Form result = null;
            foreach (FQDNN_Form form in forms)
            {
                if (form.Locale == locale)
                {
                    result = form;
                    break;
                }
            }

            if (result == null)
            {
                foreach (FQDNN_Form form in forms)
                {
                    if (form.Locale == portalDefaultLocale)
                    {
                        result = form;
                        break;
                    }
                }
            }

            return result;
        }

        public FQDNN_FormPage GetFormPage(int formID, int pageNumber)
        {
            FQDNNEntities context = GetContext();

            var Page = from c in context.FQDNN_FormPage
                        where c.FQDNN_Form.FormID == formID &&
                            c.PageNumber == pageNumber
                        select c;
            return Page.FirstOrDefault();
        }

        //get a specific form along with all the related information (pages,controls,settings,etc)
        public FQDNN_Form GetFullForm(int formID)
        {
            string _cacheName = "FQDNN_FullForm_" + formID.ToString();
            FQDNN_Form result = (FQDNN_Form)DataCache.GetCache(_cacheName);

            if (result == null)
            {
                FQDNNEntities context = GetContext();

                var Form = from c in context.FQDNN_Form.Include("FQDNN_FormPage")
                               .Include("FQDNN_FormPage.FQDNN_FormControl")
                               .Include("FQDNN_FormPage.FQDNN_FormControl.FQDNN_FormControlValue")
                               .Include("FQDNN_FormPage.FQDNN_FormControl.FQDNN_ControlDefinition")
                               .Include("FQDNN_FormSetting")
                           where c.FormID == formID
                           select c;

                result = Form.FirstOrDefault();
                if (result != null)
                {
                    DataCache.SetCache(_cacheName, result);
                }
            }
            return result;
        }

        public FQDNN_FormFiling GetLastFiling(int userID, string ipAddress, int formID)
        {
            FQDNNEntities context = GetContext();
            
            //TODO: filter by formid

            var filingQuery = from f in context.FQDNN_FormFiling
                              where (f.UserID == userID && userID > 0) || (userID <= 0 && f.IPAddress == ipAddress)
                              orderby f.DateCreated descending 
                              select f;

            FQDNN_FormFiling result = filingQuery.Take(1).FirstOrDefault();
            return result;
        }

        public FQDNN_FormFiling InsertNewFiling(string browser, string ipAddress, bool isComplete, string platform, int userID, int formID)
        {
            FQDNNEntities context = GetContext();

            FQDNN_Form form = (from f in context.FQDNN_Form 
                              where f.FormID == formID
                              select f).Single();

            FQDNN_FormFiling newFiling = new FQDNN_FormFiling();
            newFiling.Browser = browser;
            newFiling.DateCreated = DateTime.Now;
            newFiling.DateUpdated = DateTime.Now;
            newFiling.IPAddress = ipAddress;
            newFiling.IsComplete = isComplete;
            newFiling.Platform = platform;
            newFiling.UserID = userID;
            newFiling.FQDNN_Form = form;

            context.SaveChanges();

            return newFiling;
        }

        public void InsertFilingRecords(List<FQDNN_FormFilingRecord> oFilingRecords, int formFilingID)
        {
            FQDNNEntities context = GetContext();

            FQDNN_FormFiling formFiling = (from f in context.FQDNN_FormFiling
                               where f.FormFilingID == formFilingID
                               select f).Single();

            foreach (FQDNN_FormFilingRecord oFilingRecord in oFilingRecords)
            {
                oFilingRecord.FQDNN_FormFiling = formFiling;
                context.AddToFQDNN_FormFilingRecord(oFilingRecord);
            }

            context.SaveChanges();
        }
    }
}