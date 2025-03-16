namespace RPG.Utils
{
    public class LazyValue<T>
    {
        T _value;
        bool _initialized = false;
        InitializerDelegate _initializer;

        public delegate T InitializerDelegate();

        public LazyValue(InitializerDelegate initializer)
        {
            _initializer = initializer;
        }

        public T value
        {
            get
            {
                ForceInit();
                return _value;
            }
            set
            {
                _initialized = true;
                _value = value;
            }
        }

        public void ForceInit()
        {
            if(!_initialized)
            {
                _value = _initializer();
                _initialized = true;
            }
        }
    }
}