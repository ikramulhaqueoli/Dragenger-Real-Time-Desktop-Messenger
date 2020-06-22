using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ResourceLibrary;
using EntityLibrary;
using StandardAssuranceLibrary;
using FileIOAccess;

namespace CorePanels
{
    public partial class LoginPanel : Panel
    {
        private bool OldPasswordValid { set; get; }
        private bool NewPasswordValid { set; get; }
        private bool ConfirmPasswordValid { set; get; }

        private Label oldPasswordBoxSign, oldPasswordBoxErrorMessage;
        private Label newPasswordBoxSign, newPasswordBoxMessage;
        private Label confirmPasswordBoxSign, confirmPasswordBoxMessage;
        private Label passwordSetupButtonMessage;

        private string newPassword;

        private void InitializeKeySetupErrorShowers()
        {
            this.oldPasswordBoxSign = new Label();
            this.oldPasswordBoxSign.Size = new Size(this.oldPasswordBoxSign.Height / 2, this.oldPasswordBoxSign.Height / 2);
            this.oldPasswordBoxSign.Top = this.oldPasswordBox.Top + (this.oldPasswordBox.Height - oldPasswordBoxSign.Height) / 2;
            this.oldPasswordBoxSign.Left = this.oldPasswordBoxSign.Right + 3;
            this.Controls.Add(this.oldPasswordBoxSign);
            this.oldPasswordBoxErrorMessage = new Label();
            this.oldPasswordBoxErrorMessage.Font = CustomFonts.Smallest;
            this.oldPasswordBoxErrorMessage.ForeColor = Colors.ErrorTextColor;
            this.oldPasswordBoxErrorMessage.Top = this.oldPasswordBox.Bottom + 1;
            this.Controls.Add(this.oldPasswordBoxErrorMessage);

            this.newPasswordBoxSign = new Label();
            this.newPasswordBoxSign.Size = new Size(this.newPasswordBox.Height / 2, this.newPasswordBox.Height / 2);
            this.newPasswordBoxSign.Top = this.newPasswordBox.Top + (this.newPasswordBox.Height - newPasswordBoxSign.Height) / 2;
            this.newPasswordBoxSign.Left = this.newPasswordBox.Right + 3;
            this.Controls.Add(this.newPasswordBoxSign);
            this.newPasswordBoxMessage = new Label();
            this.newPasswordBoxMessage.Font = CustomFonts.Smallest;
            this.newPasswordBoxMessage.ForeColor = Colors.ErrorTextColor;
            this.newPasswordBoxMessage.Top = this.newPasswordBox.Bottom + 1;
            this.Controls.Add(this.newPasswordBoxMessage);

            this.confirmPasswordBoxSign = new Label();
            this.confirmPasswordBoxSign.Size = new Size(this.confirmPasswordBox.Height / 2, this.confirmPasswordBox.Height / 2);
            this.confirmPasswordBoxSign.Top = this.confirmPasswordBox.Top + (this.confirmPasswordBox.Height - confirmPasswordBoxSign.Height) / 2;
            this.confirmPasswordBoxSign.Left = this.confirmPasswordBox.Right + 3;
            this.Controls.Add(this.confirmPasswordBoxSign);
            this.confirmPasswordBoxMessage = new Label();
            this.confirmPasswordBoxMessage.Font = CustomFonts.Smallest;
            this.confirmPasswordBoxMessage.ForeColor = Colors.ErrorTextColor;
            this.confirmPasswordBoxMessage.Top = this.confirmPasswordBox.Bottom + 1;
            this.Controls.Add(this.confirmPasswordBoxMessage);

            this.passwordSetupButtonMessage = new Label();
            this.passwordSetupButtonMessage.Font = CustomFonts.Smallest;
            this.passwordSetupButtonMessage.TextAlign = ContentAlignment.TopRight;
            this.passwordSetupButtonMessage.ForeColor = Colors.ErrorTextColor;
            this.passwordSetupButtonMessage.Top = this.passwordSetupButton.Bottom + 3;
            this.passwordSetupButtonMessage.Visible = false;
            this.Controls.Add(this.passwordSetupButtonMessage);
        }

        private void SetOldPasswordValidity()
        {
            this.OldPasswordValid = false;
            if(this.oldPasswordString == null || this.oldPasswordString.Length == 0 || User.LoggedIn == null)
            {
                OldPasswordValid = true;
                return;
            }
            string errorMessage = Checker.CheckOldPasswordMatch(this.oldPasswordString);
            if (errorMessage == null) OldPasswordValid = true;

            if (!OldPasswordValid)
            {
                if(Universal.ParentForm.InvokeRequired)
                {
                    Universal.ParentForm.Invoke(new Action(() =>
                    {
                        this.oldPasswordBoxErrorMessage.Text = errorMessage;
                        this.oldPasswordBoxErrorMessage.Size = this.oldPasswordBoxErrorMessage.PreferredSize;
                        this.oldPasswordBoxErrorMessage.Left = this.oldPasswordBox.Right - this.oldPasswordBoxErrorMessage.Width + 5;
                        this.oldPasswordBoxSign.Image = new Bitmap(FileResources.Icon("redwarning.png"), oldPasswordBoxSign.Size);
                        this.oldPasswordBoxErrorMessage.Visible = true;
                        this.oldPasswordBoxSign.Visible = true;
                    }));
                }
                else
                {
                    this.oldPasswordBoxErrorMessage.Text = errorMessage;
                    this.oldPasswordBoxErrorMessage.Size = this.oldPasswordBoxErrorMessage.PreferredSize;
                    this.oldPasswordBoxErrorMessage.Left = this.oldPasswordBox.Right - this.oldPasswordBoxErrorMessage.Width + 5;
                    this.oldPasswordBoxSign.Image = new Bitmap(FileResources.Icon("redwarning.png"), oldPasswordBoxSign.Size);
                    this.oldPasswordBoxErrorMessage.Visible = true;
                    this.oldPasswordBoxSign.Visible = true;
                }
            }
        }

        private void SetNewPasswordValidity(Object sender, EventArgs e)
        {
            this.NewPasswordValid = false;
            TextBox newKeyBox = (TextBox)sender;
            if (newKeyBox.TextLength > 0)
            {
                string input = ((TextBox)sender).Text;
                int strength = Checker.DeterminePasswordStrength(input);
                string message;
                if (strength == -1) { message = "contains invalid characters"; this.newPasswordBoxMessage.ForeColor = Colors.ErrorTextColor; }
                if (strength == -2) { message = "maximum 25 characters allowed"; this.newPasswordBoxMessage.ForeColor = Colors.ErrorTextColor; }
                else if (strength <= 2) { message = "Too weak!\r\ntry combination of\r\nlower, upper-case, numeric, special characters"; this.newPasswordBoxMessage.ForeColor = Colors.ErrorTextColor; }
                else if (strength == 3) { message = "Medium"; this.newPasswordBoxMessage.ForeColor = Color.FromArgb(255, 153, 51); }
                else if (strength == 4) { message = "Strong"; this.newPasswordBoxMessage.ForeColor = Color.FromArgb(0, 132, 0); }
                else { message = "Very Strong"; this.newPasswordBoxMessage.ForeColor = Color.FromArgb(0, 102, 0); }
                if (strength >= 3) this.NewPasswordValid = true;
                this.newPasswordBoxSign.Visible = true;
                this.newPasswordBoxMessage.Visible = true;
                this.newPasswordBoxMessage.Text = message;
                this.newPasswordBoxMessage.TextAlign = ContentAlignment.MiddleRight;
                this.newPasswordBoxMessage.Size = this.newPasswordBoxMessage.PreferredSize;
                this.newPasswordBoxMessage.Left = this.newPasswordBox.Right - this.newPasswordBoxMessage.Width + 5;
                if (NewPasswordValid)
                {
                    this.newPassword = newKeyBox.Text;
                    this.newPasswordBoxSign.Image = new Bitmap(FileResources.Icon("ok.png"), newPasswordBoxSign.Size);
                }
                else
                {
                    this.newPassword = null;
                    this.newPasswordBoxSign.Image = new Bitmap(FileResources.Icon("redwarning.png"), newPasswordBoxSign.Size);
                }
            }
            else
            {
                this.newPasswordBoxSign.Visible = false;
                this.newPasswordBoxMessage.Visible = false;
            }
            if (this.passwordSetupButtonMessage.Visible) this.AllKeySetupConstraintsOk();
        }

        private void SetConfirmPasswordValidity(Object sender, EventArgs e)
        {
            this.ConfirmPasswordValid = false;
            TextBox confirmKeyBox = (TextBox)sender;
            if (confirmKeyBox.TextLength > 0)
            {
                string input = this.confirmPasswordBox.Text;
                this.ConfirmPasswordValid = (this.newPassword == confirmKeyBox.Text);
                string message = null;
                if (this.newPassword == null) { message = "Enter a valid password first"; this.confirmPasswordBoxMessage.ForeColor = Colors.ErrorTextColor; }
                else if (ConfirmPasswordValid) { this.confirmPasswordBoxMessage.Visible = false; }
                else { message = "Password doesn't match"; this.confirmPasswordBoxMessage.ForeColor = Colors.ErrorTextColor; }
                this.confirmPasswordBoxMessage.Text = message;
                this.confirmPasswordBoxMessage.Size = this.confirmPasswordBoxMessage.PreferredSize;
                this.confirmPasswordBoxMessage.Left = this.confirmPasswordBox.Right - this.confirmPasswordBoxMessage.Width + 5;
                this.confirmPasswordBoxMessage.Visible = true;
                this.confirmPasswordBoxSign.Visible = true;
                if (ConfirmPasswordValid)
                {
                    this.confirmPasswordBoxSign.Image = new Bitmap(FileResources.Icon("ok.png"), confirmPasswordBoxSign.Size);
                }
                else
                {
                    this.confirmPasswordBoxSign.Image = new Bitmap(FileResources.Icon("redwarning.png"), confirmPasswordBoxSign.Size);
                }
            }
            else
            {
                confirmPasswordBoxSign.Visible = false;
                confirmPasswordBoxMessage.Visible = false;
            }
            if (this.passwordSetupButtonMessage.Visible) this.AllKeySetupConstraintsOk();
        }

        public bool AllKeySetupConstraintsOk()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(
                    delegate
                    {
                        this.passwordSetupButtonMessage.Visible = false;
                    }));
            }
            else this.passwordSetupButtonMessage.Visible = false;
            this.SetOldPasswordValidity();
            string errorMessage = null;
            if (!this.OldPasswordValid) errorMessage = "Current Password is Incorrect";
            if (!this.NewPasswordValid)
            {
                if (errorMessage != null) errorMessage += "\r\n";
                errorMessage += "Invalid Password format";
            }
            else if(!this.ConfirmPasswordValid)
            {
                if (errorMessage != null) errorMessage += "\r\n";
                errorMessage += "Confirmed Password not matches";
            }

            if (errorMessage == null) return true;

            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(
                    delegate
                    {
                        this.passwordSetupButtonMessage.Text = errorMessage;
                        this.passwordSetupButtonMessage.Size = this.passwordSetupButtonMessage.PreferredSize;
                        this.passwordSetupButtonMessage.Left = this.passwordSetupButton.Right - this.passwordSetupButtonMessage.Width + 5;
                        this.passwordSetupButtonMessage.Visible = true;
                        VisualizingTools.HideWaitingAnimation();
                    }));
            }
            else
            {
                this.passwordSetupButtonMessage.Text = errorMessage;
                this.passwordSetupButtonMessage.Size = this.passwordSetupButtonMessage.PreferredSize;
                this.passwordSetupButtonMessage.Left = this.passwordSetupButton.Right - this.passwordSetupButtonMessage.Width + 5;
                this.passwordSetupButtonMessage.Visible = true;
                VisualizingTools.HideWaitingAnimation();
            }
            
            return false;
        }
    }
}
