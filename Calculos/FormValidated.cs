using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Calculos
{
    public class FormValidated : Form
    {
        public ErrorProvider errorProvider;

        public bool IsValid(Control.ControlCollection controls)
        {
            bool valid = true;
            foreach (Control control in controls)
            {
                // Set focus on control
                //control.Focus();
                // Validate causes the control's Validating event to be fired,
                // if CausesValidation is True
                Validate();
                if (!string.IsNullOrEmpty(errorProvider.GetError(control)))
                {
                    DialogResult = DialogResult.None;
                    valid = false;
                }
            }
            return valid;
        }

        public void ClearErrors(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
                errorProvider.SetError(control, string.Empty);
        }

        #region Validation Types
        public void Validation_Required(Control control, string errorMessage)
        {
            // Is required
            if (control.Text.Trim() == "")
            {
                errorProvider.SetError(control, errorMessage);
                return;
            }
            // Valid
            errorProvider.SetError(control, string.Empty);
        }

        public void Validation_NumberRequired(Control control, string requiredErrorMessage, string numberErrorMessage)
        {
            if (control.Text.Trim() == "")
            {
                errorProvider.SetError(control, requiredErrorMessage);
                return;
            }
            Validation_Number(control, numberErrorMessage);
        }

        public void Validation_Number(Control control, string errorMessage)
        {
            // Only numbers
            foreach (char c in control.Text.Trim())
                if (!Char.IsDigit(c))
                {
                    errorProvider.SetError(control, errorMessage);
                    return;
                }
            // Valid
            errorProvider.SetError(control, string.Empty);
        }

        public void Validation_NumberVinculated(Control control, string errorMessage, Control vinculatedControl, string vinculatedErrorMessage)
        {
            // Only numbers
            foreach (char c in control.Text.Trim())
                if (!Char.IsDigit(c))
                {
                    errorProvider.SetError(control, errorMessage);
                    Validation_Required(vinculatedControl, vinculatedErrorMessage);
                    return;
                }
            // Valid
            errorProvider.SetError(control, string.Empty);
            errorProvider.SetError(vinculatedControl, string.Empty);
        }

        public void Validation_Regex(Control control, string pattern, string errorMessage)
        {
            // Validation
            if (!string.IsNullOrEmpty(control.Text.Trim()))
                if (!Regex.IsMatch(control.Text.Trim(), pattern, RegexOptions.IgnoreCase))
                {
                    errorProvider.SetError(control, errorMessage);
                    return;
                }
            // Valid
            errorProvider.SetError(control, string.Empty);
        }

        public void Validation_RegexVinculated(Control control, string pattern, string errorMessage, Control vinculatedControl, string vinculatedErrorMessage)
        {
            // Validation
            if (!string.IsNullOrEmpty(control.Text.Trim()))
                if (!Regex.IsMatch(control.Text.Trim(), pattern, RegexOptions.IgnoreCase))
                {
                    errorProvider.SetError(control, errorMessage);
                    Validation_Required(vinculatedControl, vinculatedErrorMessage);
                    return;
                }
            // Valid
            errorProvider.SetError(control, string.Empty);
            errorProvider.SetError(vinculatedControl, string.Empty);
        }

        public void Validation_Email(Control control, string errorMessage)
        {
            Validation_Regex(control, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", errorMessage);
        }

        public void Validation_EmailVinculated(Control control, string errorMessage, Control vinculatedControl, string vinculatedErrorMessage)
        {
            Validation_RegexVinculated(control, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", errorMessage, vinculatedControl, vinculatedErrorMessage);
        }

        public void Validation_Price(Control control, string errorMessage)
        {
            Validation_Regex(control, @"^\d+(\" + CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator + @"\d+)?$", "Ingrese un precio valido");
            //Validation_Regex(control, @"^(?!0,?\d)(?:\d{1,3}(?:([, .])\d{3})?(?:\1\d{3})*|(?:\d+))((?!\1)[,.]\d{2})?$", "Ingrese un precio valido");
        }

        public void Validation_PriceRequired(Control control, string requiredErrorMessage, string errorMessage)
        {
            if (control.Text.Trim() == "")
            {
                errorProvider.SetError(control, requiredErrorMessage);
                return;
            }
            control.Text = control.Text.Replace(',', CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]).Replace('.', CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]);
            Validation_Price(control, errorMessage);
        }

        #endregion
    }
}
