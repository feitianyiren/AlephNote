﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace AlephNote.PluginInterface
{
	public interface INote
	{
		event NoteChangedEventHandler OnChanged;

		XElement Serialize();
		void Deserialize(XElement input);

		string GetUniqueName();
		bool EqualID(INote clonenote);

		void SetDirty();
		void SetLocalDirty();
		void SetRemoteDirty();
		void ResetLocalDirty();
		void ResetRemoteDirty();

		void OnAfterUpload(INote clonenote);
		void ApplyUpdatedData(INote other);

		ObservableCollection<string> Tags { get; }
		string Text { get; set; }
		string Title { get; set; }
		bool IsLocalSaved { get; set; }
		bool IsRemoteSaved { get; set; }
		bool IsConflictNote { get; set; }
		DateTimeOffset CreationDate { get; set; }
		DateTimeOffset ModificationDate { get; set; }

		INote Clone();
		IDisposable SuppressDirtyChanges();
		void TriggerOnChanged(bool doNotSendChangeEvents);
	}
}