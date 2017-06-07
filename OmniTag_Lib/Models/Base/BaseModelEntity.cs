using System;
using System.ComponentModel;
using NCGLib;

namespace OmniTag.Models.Base
{
	public abstract class BaseModelEntity : IModelEntity
	{
		protected readonly PropertyChangeNotifier PropNotify;

		protected BaseModelEntity()
		{
			PropNotify = new PropertyChangeNotifier();
			PropNotify.PropertyChanging += (sender, args) => OnPropertyChanging(args.PropertyName);
			PropNotify.PropertyChanged += (sender, args) => OnPropertyChanged(args.PropertyName);
		}

		private int _id;
		public int Id
		{
			get { return _id; }
			set { PropNotify.SetProperty(ref _id, value); }
		}

		private DateTime _dateCreated;
		public DateTime DateCreated
		{
			get { return _dateCreated; }
			set { PropNotify.SetProperty(ref _dateCreated, value); }
		}

		private DateTime _lastModifiedDate;
		public DateTime LastModifiedDate
		{
			get { return _lastModifiedDate; }
			set { PropNotify.SetProperty(ref _lastModifiedDate, value); }
		}

		private DateTime? _dateDeleted;
		public DateTime? DateDeleted
		{
			get { return _dateDeleted; }
			set { PropNotify.SetProperty(ref _dateDeleted, value); }
		}
		
		#region Property Changing
		public event PropertyChangingEventHandler PropertyChanging;
		protected virtual void OnPropertyChanging(string propertyName = null)
		{
			var propChanging = PropertyChanging;
			if (propChanging != null)
				propChanging(this, new PropertyChangingEventArgs(propertyName));
		}
		#endregion

		#region Property Changed
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName = null)
		{
			var propChanged = PropertyChanged;
			if (propChanged != null)
				propChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}
