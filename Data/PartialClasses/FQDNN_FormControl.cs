namespace HristoEvtimov.DNN.Modules.FormsQuestionnaireDNN.Data
{
    public partial class FQDNN_FormControl
    {
        public enum DataSourceTypes
        {
            None = 1,
            Static = 2,
            Dynamic = 3,
        }

        public string GetControlID()
        {
            string result = "";

            foreach (char c in _Name)
            {
                if ((c >= 'a' && c <= 'z') ||
                    (c >= 'A' && c <= 'Z') ||
                    c == '_')
                {
                    result += c;
                }
            }

            result = result + "_" + _FormControlID;

            return result;
        }
    }
}