using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using ReactiveUI;

namespace Task2
{
    public class ReflectionViewModel : ViewModelBase
    {
        private string _dllPath = string.Empty;
        public string DllPath
        {
            get => _dllPath;
            set => this.RaiseAndSetIfChanged(ref _dllPath, value);
        }

        public ObservableCollection<TypeDisplay> FoundTypes { get; } = new();
        private TypeDisplay? _selectedType;
        public TypeDisplay? SelectedType
        {
            get => _selectedType;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedType, value);
                LoadMethods();
            }
        }

        public ObservableCollection<MethodDisplay> Methods { get; } = new();
        private MethodDisplay? _selectedMethod;
        public MethodDisplay? SelectedMethod
        {
            get => _selectedMethod;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedMethod, value);
                LoadParameters();
            }
        }

        public ObservableCollection<ParameterViewModel> Parameters { get; } = new();
        private string _result = string.Empty;
        public string Result
        {
            get => _result;
            set => this.RaiseAndSetIfChanged(ref _result, value);
        }

        public ICommand LoadAssemblyCommand { get; }
        public ICommand ExecuteCommand { get; }

        public ReflectionViewModel()
        {
            LoadAssemblyCommand = new RelayCommand(LoadAssembly);
            ExecuteCommand = new RelayCommand(Execute);
        }

        private void LoadAssembly()
        {
            FoundTypes.Clear();
            Methods.Clear();
            Parameters.Clear();
            Result = string.Empty;
            if (string.IsNullOrWhiteSpace(DllPath)) return;
            try
            {
                var asm = Assembly.LoadFrom(DllPath);
                var baseType = typeof(Aircraft); // Можно заменить на интерфейс
                var types = asm.GetTypes().Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract);
                foreach (var t in types) FoundTypes.Add(new TypeDisplay(t));
            }
            catch (Exception ex)
            {
                Result = $"Ошибка загрузки: {ex.Message}";
            }
        }

        private void LoadMethods()
        {
            Methods.Clear();
            Parameters.Clear();
            if (SelectedType == null) return;
            foreach (var m in SelectedType.Type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                Methods.Add(new MethodDisplay(m));
        }

        private void LoadParameters()
        {
            Parameters.Clear();
            if (SelectedMethod == null) return;
            foreach (var p in SelectedMethod.Method.GetParameters())
                Parameters.Add(new ParameterViewModel(p));
        }

        private void Execute()
        {
            if (SelectedType == null || SelectedMethod == null) return;
            try
            {
                var obj = Activator.CreateInstance(SelectedType.Type);
                var paramValues = Parameters.Select(p => p.GetValue()).ToArray();
                var result = SelectedMethod.Method.Invoke(obj, paramValues);
                Result = result?.ToString() ?? "void";
            }
            catch (Exception ex)
            {
                Result = $"Ошибка выполнения: {ex.Message}";
            }
        }
    }

    public class ParameterViewModel : ViewModelBase
    {
        public ParameterInfo Info { get; }
        private string _input = string.Empty;
        public string Input
        {
            get => _input;
            set => this.RaiseAndSetIfChanged(ref _input, value);
        }
        public string Name => Info.Name;
        public string TypeName => Info.ParameterType.Name;
        public ParameterViewModel(ParameterInfo info)
        {
            Info = info;
        }
        public object? GetValue()
        {
            try
            {
                if (Info.ParameterType == typeof(int))
                    return int.Parse(Input);
                if (Info.ParameterType == typeof(double))
                    return double.Parse(Input);
                if (Info.ParameterType == typeof(string))
                    return Input;
                // Добавьте другие типы по необходимости
                return null;
            }
            catch
            {
                return null;
            }
        }
    }

    public class TypeDisplay
    {
        public string Display { get; }
        public Type Type { get; }
        public TypeDisplay(Type type)
        {
            Type = type;
            Display = type.FullName ?? type.Name;
        }
    }

    public class MethodDisplay
    {
        public string Display { get; }
        public MethodInfo Method { get; }
        public MethodDisplay(MethodInfo method)
        {
            Method = method;
            Display = method.Name;
        }
    }
}
