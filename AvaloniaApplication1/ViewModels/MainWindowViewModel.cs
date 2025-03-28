using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AvaloniaApplication1.Models;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly MyQueue<string> _queue;
    private string _newItem;

    public MainWindowViewModel()
    {
        _queue = new MyQueue<string>();
        _newItem = string.Empty;
        _errorMessage = string.Empty;
        Items = new ObservableCollection<string>();
    }

    public ObservableCollection<string> Items { get; set; }

    private string _errorMessage;

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public string NewItem
    {
        get => _newItem;
        set => SetProperty(ref _newItem, value);
    }

    public ICommand EnqueueCommand => new RelayCommand<object>(param =>
    {
        var item = param?.ToString();
        if (string.IsNullOrEmpty(item))
        {
            return;
        }

        _queue.Enqueue(item);
        NewItem = string.Empty;
        ErrorMessage = string.Empty;
        UpdateItems();
    });

    public ICommand DequeueCommand => new RelayCommand(() =>
        {
            try
            {
                _queue.Dequeue();
                UpdateItems();
                ErrorMessage = string.Empty;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }
    );

    public ICommand ClearCommand => new RelayCommand(() =>
    {
        _queue.Clear();
        ErrorMessage = string.Empty;
        UpdateItems();
    });

    private void UpdateItems()
    {
        Items = new ObservableCollection<string>(_queueSnapshot());
        OnPropertyChanged(nameof(Items));
        OnPropertyChanged(nameof(CurrentItem));
        OnPropertyChanged(nameof(IsEmpty));
    }

    public string CurrentItem => !IsEmpty ? _queue.Current : "Queue is empty";
    public bool IsEmpty => _queue.IsEmpty;

    private IEnumerable<string> _queueSnapshot()
    {
        var tempQueue = new Queue<string>();
        while (!_queue.IsEmpty)
        {
            var item = _queue.Dequeue();
            tempQueue.Enqueue(item);
            yield return item;
        }

        while (tempQueue.Count > 0)
        {
            _queue.Enqueue(tempQueue.Dequeue());
        }
    }
}