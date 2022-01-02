using System;
using System.Windows.Forms;

namespace CreateNameplate
{
    /// <summary>
    /// Subclass ConfigWindow of BaseClass Form. 
    /// There you can enter your configuration details of your project.
    /// </summary>
    public partial class ConfigWindow : Form
    {
        /// <summary>
        /// The REST-Endpoint of the AAS Server. 
        /// </summary>
        public static string url = null;

        /// <summary>
        /// The name of the active project in which the nameplate should be created. 
        /// </summary>
        public static string projectName = null;

        /// <summary>
        /// Constructor for the ConfigWindow. It is just initializing.
        /// </summary>
        public ConfigWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Method is called when clicking on the Cancel-Button. 
        /// Because of the Cancel-Context nothing happens, the form just closes. 
        /// </summary>
        /// <param name="sender">the parameter sender is not used. </param>
        /// <param name="e">The parameter EventArgs is not used. </param>
        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Method is called when clicking on the Send-Button.
        /// It stores then the Text of both Boxes (textBoxAAS and textBoxProjectName) globally. After that the form closes. 
        /// </summary>
        /// <param name="sender">the parameter sender is not used. </param>
        /// <param name="e">The parameter EventArgs is not used. </param>
        private void ButtonSend_Click(object sender, EventArgs e)
        {
            url = this.textBoxAAS.Text;
            projectName = this.textBoxProjectName.Text;
            this.Close();
        }
    }
}
