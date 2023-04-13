//------------------------------------------------------------------------------
// <copyright>
//   Copyright (c) 2005-2006 Nesterovsky Bros. All rights reserved.
// </copyright>
// <description>
//   Undo and redo manager.
// </description>
// <history>
//  $Modtime: 29/03/06 19:23 $
//  $Revision: 16 $
//  $Logfile: /Actions/UndoRedoManager.cs $
// </history>
//------------------------------------------------------------------------------
//https://github.com/git-thinh/UndoRedo
namespace NesterovskyBros.Actions
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// Undo and redo manager.
  /// </summary>
  public class UndoRedoManager
  {
    /// <summary>
    /// Creates undo and redo scope.
    /// </summary>
    public struct Scope: IDisposable
    {
      private ScopeAction scopeAction;
      private UndoRedoManager undoRedoManager;
      private EventHandler complete;

      /// <summary>
      /// Create an instance of the undo and redo scope.
      /// </summary>
      /// <param name="undoRedoManager">Instance of the undo and redo manager.</param>
      /// <param name="name">Scope name.</param>
      public Scope(UndoRedoManager undoRedoManager, string name):
        this(undoRedoManager, name, null)
      {
      }

      /// <summary>
      /// Create an instance of the undo and redo scope.
      /// </summary>
      /// <param name="undoRedoManager">Instance of the undo and redo manager.</param>
      /// <param name="name">Scope name.</param>
      /// <param name="complete">Action complete handler.</param>
      public Scope(UndoRedoManager undoRedoManager, string name, EventHandler complete)
      {
        if (undoRedoManager == null)
          throw new ArgumentNullException("undoRedoManager");

        this.undoRedoManager = undoRedoManager;
        this.scopeAction = undoRedoManager.StartScope(name);
        this.complete = complete;

        if (complete != null)
          undoRedoManager.Run(new DelegateAction(UndoScope, complete));
      }

      /// <summary>
      /// Ends undo and redo scope.
      /// </summary>
      public void Dispose()
      {
        if (complete != null)
          undoRedoManager.Run(new DelegateAction(DoScope, complete));

        undoRedoManager.EndScope(scopeAction);
      }

      private static void DoScope(object sender, ActionEventArgs eventArgs)
      {
        if (eventArgs.Type == ActionType.Undo)
          return;

        EventHandler eventHandler = eventArgs.Value as EventHandler;

        if (eventHandler != null)
          eventHandler(eventArgs.UndoRedoManager, eventArgs);
      }

      private static void UndoScope(object sender, ActionEventArgs eventArgs)
      {
        if (eventArgs.Type != ActionType.Undo)
          return;

        EventHandler eventHandler = eventArgs.Value as EventHandler;

        if (eventHandler != null)
          eventHandler(eventArgs.UndoRedoManager, eventArgs);
      }
    }

    struct Action
    {
      public IAction prime;
      public List<IAction> list;
    }

    class ScopeAction: IAction
    {
      private string name;

      public ScopeAction(string name)
      {
        this.name = name;
      }

      public string Name
      {
        get { return name; }
      }

      public bool Run(UndoRedoManager undoRedoManager, ActionType type)
      {
        return true;
      }
    }

    const int defaultCapacity = 10000;

    private int capacity;
    private UndoRedoManagerState state;
    private List<Action> actions;
    private int redoPosition;
    private int recursion;
    private Action current;

    private List<IAction> mergeList;

    /// <summary>
    /// Creates an instance of the UndoRedoManager.
    /// </summary>
    public UndoRedoManager(): this(defaultCapacity) { }

    /// <summary>
    /// Creates an instance of the UndoRedoManager.
    /// </summary>
    /// <param name="capacity">Capacity of undo and redo list.</param>
    public UndoRedoManager(int capacity)
    {
      this.capacity = capacity > 0 ? capacity : defaultCapacity;
      this.actions = new List<Action>();
      this.mergeList = new List<IAction>(1);
    }

    /// <summary>
    /// Event that occurs when the state of the undo redo manager is changed.
    /// </summary>
    public event EventHandler Changed;

    /// <summary>
    /// Gets current state of the undo redo manager.
    /// </summary>
    public UndoRedoManagerState State
    {
      get { return state; }
    }

    /// <summary>
    /// Indicates whether UndoRedoManager can run an action.
    /// </summary>
    public bool CanRun
    {
      get 
      {
        switch(state)
        { 
          case UndoRedoManagerState.Idle:
          case UndoRedoManagerState.ProcessDo:
            return true;
          default:
            return false;
        }
      }
    }

    /// <summary>
    /// Runs an action and puts it in the undo list.
    /// </summary>
    /// <param name="action">action to run</param>
    public void Run(IAction action)
    {
      if (action == null)
        throw new ArgumentNullException("action");

      bool changed = false;

      recursion++;

      try
      {
        switch (state)
        {
          case UndoRedoManagerState.Idle:
            changed = InternalClearRedo();
            state = UndoRedoManagerState.ProcessDo;
            current.prime = null;
            current.list = new List<IAction>();

            break;
          case UndoRedoManagerState.ProcessDo:
            break;
          default:
            throw new InvalidOperationException();
        }

        ScopeAction scope = action as ScopeAction;

        if (action.Run(this, ActionType.Do))
        {
          IAction item = action;

          Merge(current.list, ref item);

          if ((item != null) && !IsSecondary(item))
          {
            if ((current.prime == null) || (recursion == 1) && (scope == null))
              current.prime = item;
          }
        }

        if (recursion != 1)
          return;

        if (current.prime == null)
        {
          // There is no prime action.
          // We can cancel whole action.
          UndoCurrent();

          return;
        }

        bool hasPrimary = false;

        for(int i = 0, c = current.list.Count; i < c; i++)
        {
          IAction item = current.list[i];

          if ((current.prime != item) && !IsSecondary(item))
          {
            hasPrimary = true;

            break;
          }
        }

        // End of scope.
        if (!hasPrimary && (scope != null))
        {
          UndoCurrent();

          return;
        }

        changed = true;

        bool addAction = true;

        if (!hasPrimary && (actions.Count != 0))
        {
          // Merge previous action with a new one.
          Action previousAction = actions[actions.Count - 1];

          IMergeableAction mergeableAction = previousAction.prime as IMergeableAction;

          if (mergeableAction != null)
          {
            IAction mergedAction;

            mergeList.Add(current.prime);

            try
            {
              mergedAction = mergeableAction.Merge(this, mergeList);
              addAction = mergeList.Count != 0;

              int index = current.list.IndexOf(current.prime);

              if (addAction)
              {
                current.prime = mergeList[0];

                if (index != -1)
                  current.list[index] = current.prime;
              }
              else
              {
                current.prime = mergedAction;
                current.list.RemoveAt(index);
              }
            }
            finally
            {
              mergeList.Clear();
            }

            if (!addAction)
            {
              if (previousAction.list != null)
              {
                Merge(previousAction.list, current.list);
                current.list = previousAction.list;
              }

              if (current.prime == null)
              {
                UndoCurrent();
                actions.RemoveAt(actions.Count - 1);
              }
              else
              {
                if (current.list.Count < 2)
                  current.list = null;

                actions[actions.Count - 1] = current;
              }

              return;
            }
          }
        }

        if (addAction)
        {
          if (current.list.Count < 2)
            current.list = null;

          actions.Add(current);
        }
      }
      finally
      {
        if (--recursion == 0)
        {
          state = UndoRedoManagerState.Idle;
          redoPosition = actions.Count;

          if (CheckUndoListSize())
            changed = true;

          if (changed)
            OnChanged();
        }
      }
    }

    /// <summary>
    /// Gets action name for the item in the undo redo list.
    /// </summary>
    /// <param name="index">Index of the action.</param>
    /// <returns>Action name.</returns>
    public string this[int index]
    {
      get { return actions[index].prime.Name; }
    }

    /// <summary>
    /// Indicates whether undo operation is available.
    /// </summary>
    public bool CanUndo
    {
      get { return (State == UndoRedoManagerState.Idle) && (RedoPosition > 0); }
    }

    /// <summary>
    /// Indicates whether redo operation is available.
    /// </summary>
    public bool CanRedo
    {
      get { return (State == UndoRedoManagerState.Idle) && (RedoPosition < Count); }
    }

    /// <summary>
    /// Gets count of items in the undo redo list.
    /// </summary>
    public int Count
    {
      get { return actions.Count; }
    }

    /// <summary>
    /// Gets redo position.
    /// </summary>
    public int RedoPosition
    {
      get { return redoPosition; }
    }

    /// <summary>
    /// Indicates whether Clear operation is available.
    /// </summary>
    public bool CanClear
    {
      get { return State == UndoRedoManagerState.Idle; }
    }

    /// <summary>
    /// Clears undo list.
    /// </summary>
    public void Clear()
    {
      if (!CanClear)
        throw new InvalidOperationException();

      InternalClear();
    }

    /// <summary>
    /// Clears redo actions.
    /// </summary>
    public void ClearRedo()
    {
      if (State != UndoRedoManagerState.Idle)
        throw new InvalidOperationException();

      InternalClearRedo();
    }

    /// <summary>
    /// Undoes last action.
    /// </summary>
    public void Undo()
    {
      if (!CanUndo)
        throw new InvalidOperationException();

      state = UndoRedoManagerState.ProcessUndo;

      try
      {
        RunAction(ActionType.Undo);
      }
      finally
      {
        state = UndoRedoManagerState.Idle;
      }

      OnChanged();
    }

    /// <summary>
    /// Undoes actions up to but not including a point specified in a position.
    /// </summary>
    /// <param name="position">Undo point.</param>
    public void Undo(int position)
    { 
      if (!CanUndo)
        throw new InvalidOperationException();

      if ((position <= 0) || (position >= redoPosition))
        throw new ArgumentException("position");

      recursion++;

      try
      {
        for(int i = 0, c = redoPosition - position; i < c; i++)
        {
          Undo();
        }
      }
      finally
      {
        recursion--;
      }

      OnChanged();
    }

    /// <summary>
    /// Roes last undoed action.
    /// </summary>
    public void Redo()
    {
      if (!CanRedo)
        throw new InvalidOperationException();

      state = UndoRedoManagerState.ProcessRedo;

      try
      {
        RunAction(ActionType.Redo);
      }
      finally
      {
        state = UndoRedoManagerState.Idle;
      }

      OnChanged();
    }

    /// <summary>
    /// Redoes undoed actions up to and including a point specified in a position.
    /// </summary>
    /// <param name="position">Redo point.</param>
    public void Redo(int position)
    {
      if (!CanRedo)
        throw new InvalidOperationException();

      if ((position <= redoPosition) || (position > actions.Count))
        throw new ArgumentException("position");

      recursion++;

      try
      {
        for(int i = 0, c = position - redoPosition; i < c; i++)
        {
          Redo();
        }
      }
      finally
      {
        recursion--;
      }

      OnChanged();
    }

    /// <summary>
    /// Create undo and redo scope.
    /// This method should be compensated with Scope.Dispose call.
    /// </summary>
    /// <param name="name">Scope name.</param>
    /// <returns>Instance of the undo and redo scope.</returns>
    public Scope CreateScope(string name)
    {
      return new Scope(this, name);
    }

    /// <summary>
    /// Create undo and redo scope.
    /// This method should be compensated with Scope.Dispose call.
    /// </summary>
    /// <param name="name">Scope name.</param>
    /// <param name="complete">Scope complete handler.</param>
    /// <returns>Instance of the undo and redo scope.</returns>
    public Scope CreateScope(string name, EventHandler complete)
    {
      return new Scope(this, name, complete);
    }

    private ScopeAction StartScope(string name)
    {
      if (recursion != 0)
        return null;

      ScopeAction scopeAction = new ScopeAction(name);

      recursion = 1;

      try
      {
        Run(scopeAction);
      }
      catch
      {
        recursion = 0;
        state = UndoRedoManagerState.Idle;

        throw;
      }

      return scopeAction;
    }

    private void EndScope(ScopeAction scopeAction)
    {
      if (scopeAction == null)
        return;

      if ((recursion != 1) ||
        (current.prime != scopeAction) ||
        (state != UndoRedoManagerState.ProcessDo))
      {
        throw new InvalidOperationException();
      }

      recursion = 0;
      Run(scopeAction);
    }

    private bool InternalClear()
    {
      if (actions.Count == 0)
        return false;

      actions.Clear();
      redoPosition = 0;
      OnChanged();

      return true;
    }

    private bool InternalClearRedo()
    {
      int delta = actions.Count - redoPosition;

      if (delta <= 0)
        return false;

      actions.RemoveRange(redoPosition, delta);
      OnChanged();

      return true;
    }

    private void OnChanged()
    {
      if (recursion != 0)
        return;

      EventHandler eventHandler = Changed;

      if (eventHandler != null)
        eventHandler(this, EventArgs.Empty);
    }

    private bool CheckUndoListSize()
    {
      int delta = actions.Count - capacity;

      if (delta <= 0)
        return false;

      actions.RemoveRange(0, delta);

      return true;
    }

    private static bool IsSecondary(IAction action)
    { 
      ISecondaryAction secondaryAction = action as ISecondaryAction;

      return (secondaryAction != null) && secondaryAction.CanCancel;
    }

    private void UndoCurrent()
    {
      state = UndoRedoManagerState.ProcessUndo;

      try
      {
        for(int i = current.list.Count; i-- > 0; )
        {
          current.list[i].Run(this, ActionType.Undo);
        }
      }
      finally
      {
        state = UndoRedoManagerState.ProcessDo;
      }
    }

    private bool Merge(List<IAction> previous, ref IAction action)
    {
      mergeList.Add(action);

      try
      {
        Merge(previous, mergeList);

        if (mergeList.Count == 0)
        {
          action = null;

          return true;
        }
        else
        {
          IAction result = mergeList[0];

          if (action == result)
            return false;

          action = result;

          return true;
        }
      }
      finally
      {
        mergeList.Clear();
      }
    }

    private void Merge(List<IAction> previous, List<IAction> next)
    {
      if (previous == null)
        return;

      for(int i = previous.Count; i-- > 0;)
      {
        if (next.Count == 0)
          break;

        IAction action = previous[i];
        IMergeableAction mergeableAction = action as IMergeableAction;

        if (mergeableAction == null)
          continue;

        IAction mergedAction = mergeableAction.Merge(this, next);

        if (mergedAction == null)
          previous.RemoveAt(i);
        else if (mergedAction != action)
          previous[i] = mergedAction;
      }

      previous.AddRange(next);
    }

    private void RunAction(ActionType type)
    {
      bool changed = false;
      int index = type == ActionType.Undo ? --redoPosition : redoPosition++;
      Action action = actions[index];
      IAction prime = action.prime;

      if (action.list != null)
      {
        for(int i = 0, c = action.list.Count; i < c; i++)
        {
          int itemIndex = type == ActionType.Undo ? c - i - 1 : i;
          IAction item = action.list[itemIndex];

          if (!item.Run(this, type))
          {
            if (prime == item)
              action.prime = null;

            action.list.RemoveAt(itemIndex);

            if (type != ActionType.Undo)
            {
              i--;
              c--;
            }

            changed = true;
          }
        }
      }
      else
      {
        if (!prime.Run(this, type))
        {
          action.prime = null;
          changed = true;
        }
      }

      if (action.prime == null)
        action.prime = new ScopeAction(prime.Name);

      if (changed)
        actions[index] = action;
    }
  }
}