using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace batch_rename
{
    public class RuleTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            var method = item as RunRule;
            if (method == null)
                return null;
            if (method.Name == "Add prefix")
                return element.FindResource("template1") as DataTemplate;
            else
                return element.FindResource("template2") as DataTemplate;
        }
    }
}
