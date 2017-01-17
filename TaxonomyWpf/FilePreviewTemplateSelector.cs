using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TaxonomyWpf
{
	public class FilePreviewTemplateSelector : DataTemplateSelector
	{
		public DataTemplate NullTemplate { get; set; }

		public DataTemplate ImageTemplate { get; set; }

		public DataTemplate TextTemplate { get; set; }

		public DataTemplate BinaryTemplate { get; set; }

		public DataTemplate VideoTemplate { get; set; }

		public DataTemplate AnimatedGifTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if(item is NullPreviewModel)
				return NullTemplate;
			if(item is ImagePreviewModel)
				return ImageTemplate;
			if(item is TextFilePreviewModel)
				return TextTemplate;
			if(item is BinaryFilePreviewModel)
				return BinaryTemplate;
			if (item is VideoPreviewModel)
				return VideoTemplate;
			if (item is AnimatedGifPreviewModel)
				return AnimatedGifTemplate;
			return NullTemplate;
		}
	}
}
