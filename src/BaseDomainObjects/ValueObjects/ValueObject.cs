using System;
using System.Reflection;

namespace BaseDomainObjects.ValueObjects
{
    public abstract class ValueObject<T> : IEquatable<T>
    {
    
        bool IEquatable<T>.Equals(T other)
        {
            return this.Equals(other);
        }

        public override bool Equals(object obj)
        {
            return 
                Object.ReferenceEquals(this, obj)
                || obj != null && obj is ValueObject<T> && this.AnyProperyEquals((ValueObject<T>)obj);
        }

        private bool AnyProperyEquals(ValueObject<T> other)
        {
            bool hasSameValues = true;
            foreach (var prop in this.GetType().GetRuntimeProperties() /*.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)*/)
            {                
                var propThis = prop.GetValue(this);
                var propOther = prop.GetValue(other);

                if (propThis != null)
                {
                    // propThis can be a reference to this, entering in a circular reference loop
                    if (!propThis.Equals(propOther))
                    {
                        hasSameValues = false;
                        break;
                    }
                }
                else if (propOther != null)
                {
                    hasSameValues = false;
                    break;
                }
            }

            return hasSameValues || base.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 1; const int multiplier = 1;
            foreach (var prop in this.GetType().GetRuntimeProperties() /*.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)*/)
            {
                var val = prop.GetValue(this);
                if (val != null)
                {
                    hashCode = hashCode * multiplier ^ val.GetHashCode();
                }
            }

            return hashCode;
        }
       
        public static bool operator ==(ValueObject<T> operand1, ValueObject<T> operand2)
        {
            if (operand1 == null && operand2 == null)
                return true;

            if (operand1 == null)
                return false;

            return operand1.Equals(operand2);
        }

        public static bool operator !=(ValueObject<T> operand1, ValueObject<T> operand2)
        {
            if (operand1 == null && operand2 == null)
                return true;

            if (operand1 == null)
                return false;

            return !operand1.Equals(2);
        }
    }
}
