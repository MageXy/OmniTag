using System;
using NCGLib;

namespace OmniTag.Models.Base
{
	public abstract class BaseModelEntity : PropertyChangeNotifierEntity, IModelEntity
	{
		private int _id;
		public int Id
		{
			get { return _id; }
			set { SetProperty(ref _id, value); }
		}

		private DateTime _dateCreated;
		public DateTime DateCreated
		{
			get { return _dateCreated; }
			set { SetProperty(ref _dateCreated, value); }
		}

		private DateTime _lastModifiedDate;
		public DateTime LastModifiedDate
		{
			get { return _lastModifiedDate; }
			set { SetProperty(ref _lastModifiedDate, value); }
		}

		private DateTime? _dateDeleted;
		public DateTime? DateDeleted
		{
			get { return _dateDeleted; }
			set { SetProperty(ref _dateDeleted, value); }
		}
	}
}
