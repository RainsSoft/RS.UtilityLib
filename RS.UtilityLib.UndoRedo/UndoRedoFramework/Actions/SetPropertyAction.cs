using System.Reflection;

namespace GuiLabs.Undo
{
    /// <summary>
    /// 这是一个示例操作，可以更改任何对象上的任何属性 它还可以撤消它所做的
    /// This is a sample action that can change any property on any object
    /// It can also undo what it did
    /// </summary>
    public class SetPropertyAction : AbstractAction
    {
        public SetPropertyAction(object parentObject, string propertyName, object value) {
            ParentObject = parentObject;
            //Property = parentObject.GetType().GetTypeInfo().GetDeclaredProperty(propertyName);
            Property = parentObject.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
            //if (_PropertyInfo != null) {
            //    object _EventList = _PropertyInfo.GetValue(p_Control, null);
            //}         
            Value = value;
        }

        public object ParentObject { get; set; }
        public PropertyInfo Property { get; set; }
        public object Value { get; set; }
        public object OldValue { get; set; }

        protected override void ExecuteCore() {
            OldValue = Property.GetValue(ParentObject, null);
            Property.SetValue(ParentObject, Value, null);
        }

        protected override void UnExecuteCore() {
            Property.SetValue(ParentObject, OldValue, null);
        }

        /// <summary>
        /// 同一对象上同一属性的后续更改将合并为一个操作
        /// Subsequent changes of the same property on the same object are consolidated into one action
        /// </summary>
        /// <param name="followingAction">正在记录的后续操作.Subsequent action that is being recorded</param>
        /// <returns>如果同意与下一个操作合并，则为 true;如果应单独记录下一个操作，则为 false。
        /// true if it agreed to merge with the next action, 
        /// false if the next action should be recorded separately</returns>
        public override bool TryToMerge(IAction followingAction) {
            SetPropertyAction next = followingAction as SetPropertyAction;
            if (next != null && next.ParentObject == this.ParentObject && next.Property == this.Property) {
                Value = next.Value;
                Property.SetValue(ParentObject, Value, null);
                return true;
            }
            return false;
        }
    }
}
