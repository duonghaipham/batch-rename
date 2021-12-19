﻿using System.IO;
using System.Globalization;
using System.Windows.Controls;

namespace Contract
{
    public class FileNameRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string segment = (string)value;
            if (segment.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                return new ValidationResult(
                    false,
                    @"Can't contain any of the characters: \ / : * ? "" < > |"
                );
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
    }
}
