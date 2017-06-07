using System;
using System.ComponentModel;

namespace OmniTag.Models.Base
{
	public interface IModelEntity : INotifyPropertyChanged, INotifyPropertyChanging
	{
		int Id { get; set; }
		DateTime DateCreated { get; set; }
		DateTime LastModifiedDate { get; set; }
		DateTime? DateDeleted { get; set; }
	}
}
