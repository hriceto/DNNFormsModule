namespace HristoEvtimov.DNN.Modules.FormsQuestionnaireDNN.Data
{
    public partial class FQDNN_FormPage
    {
        public string GetControlID()
        {
            string result = "Page" + _PageNumber.ToString();
            return result;
        }

        public string GetNextPageControlID()
        {
            string result = "btnNextPage" + _PageNumber.ToString();
            return result;
        }

        public string GetPreviousPageControlID()
        {
            string result = "btnPreviousPage" + _PageNumber.ToString();
            return result;
        }
    }
}