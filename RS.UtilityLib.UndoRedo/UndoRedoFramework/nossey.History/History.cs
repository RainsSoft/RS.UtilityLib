﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace UndoRedo
{
    public static class History
    {
        #region ElementAt

        static TSource ElementAt<TSource>(this IEnumerable<TSource> source, int index) {
            if (index < 0)
                throw new ArgumentOutOfRangeException();
            //1
            var list = source as IList<TSource>;
            if (list != null)
                return list[index];
            //2
            long counter = 0L;

            foreach (var element in source) {
                if (index == counter++)
                    return element;
            }         
            throw new ArgumentOutOfRangeException();
            return default(TSource);
        }



        #endregion
        #region Declarations

        public delegate void RecordableAction();

        internal class State
        {
            public State PrevState { get; set; }
            public State NextState { get; set; }
            public Command PrevCommand { get; set; }
            public Command NextCommand { get; set; }
        }

        internal class Command
        {

            public Command() { }

            public Command(RecordableAction act, RecordableAction undo) {
                _Do = act;
                _Undo = undo;
            }

            public void executeAction() {
                _Do?.Invoke();
            }

            public void undoAction() {
                _Undo?.Invoke();
            }

            protected event RecordableAction _Do;
            protected event RecordableAction _Undo;
        }


        /// <summary>
        /// TransactionCommand packs multiple commands as one command.
        /// </summary>
        internal class TransactionCommand : Command
        {
            public TransactionCommand() {
                _Do += () => {
                    for (int i = 0; i < CommandSet.Count; ++i) {
                        CommandSet[i].executeAction();
                    }
                };
                _Undo += () => {
                    for (int i = CommandSet.Count - 1; i >= 0; --i) {
                        CommandSet[i].undoAction();
                    }
                };
            }

            public void AddCommand(Command cmd) {
                if (cmd == null) {
                    Console.WriteLine("Cannot add null command");
                    return;
                }
                CommandSet.Add(cmd);
            }

            List<Command> CommandSet = new List<Command>();
        }

        #endregion

        #region Fields

        static Object Lock = new Object();
        static List<State> States = new List<State>();
        static TransactionCommand TransCommand;

        #endregion

        #region Properties

        /// <summary>
        /// Must not be null
        /// </summary>
        static State CurrentState {
            get {
                // doesn't execute normmaly
                if (States.Count <= 0)
                    return null;

                // doesn't execute normmaly
                if (!IsInRange(0, States.Count - 1, CurrentStateIndex))
                    return null;

                var index = Clamp(0, States.Count - 1, CurrentStateIndex);
                return States.ElementAt(index);
            }
        }

        static int CurrentStateIndex {
            get {
                return _CurrentStateIndex;
            }
            set {
                if (_CurrentStateIndex != value)
                    _CurrentStateIndex = value;
            }
        }
        static int _CurrentStateIndex = 0;

        static bool Transacting {
            get {
                return _Transacting;
            }
            set {
                if (_Transacting != value)
                    _Transacting = value;
            }
        }
        static bool _Transacting = false;

        #endregion

        #region Public methods

        public static void Record(RecordableAction act, RecordableAction undo) {
            lock (Lock) {
                var cmd = new Command(act, undo);
                cmd.executeAction();
                if (Transacting) {
                    TransCommand.AddCommand(cmd);
                }
                else {
                    var newState = new State();
                    newState.PrevCommand = cmd;
                    AddNewState(newState);
                }
            }
        }

        /// <summary>
        /// Undo the last action
        /// </summary>
        public static void Undo() {
            lock (Lock) {
                var prev = CurrentState.PrevState;
                if (prev == null) {
                    Console.WriteLine("Invalid operation.");
                    return;
                }

                CurrentState.PrevCommand?.undoAction();
                CurrentStateIndex = States.IndexOf(prev);
            }
        }

        /// <summary>
        /// Redo the last action
        /// </summary>
        public static void Redo() {
            lock (Lock) {
                var next = CurrentState.NextState;
                if (next == null) {
                    Console.WriteLine("Invalid operation.");
                    return;
                }

                CurrentState.NextCommand?.executeAction();
                CurrentStateIndex = States.IndexOf(next);
            }
        }

        /// <summary>
        /// Start recording multiple actions
        /// </summary>
        static public void BeginTransaction() {
            lock (Lock) {
                TransCommand = new TransactionCommand();
                Transacting = true;
            }
        }

        /// <summary>
        /// End recording multiple actions
        /// </summary>
        static public void EndTransaction() {
            lock (Lock) {
                Transacting = false;
                var newState = new State();
                newState.PrevCommand = TransCommand;
                AddNewState(newState);
                TransCommand = null;
            }
        }

        #endregion

        #region Private methods

        static History() {
            States.Add(new State());
        }

        /// <summary>
        /// Eliminate all states starts with 'begin'
        /// </summary>
        /// <param name="begin"></param>
        static void EliminateStates(State begin) {
            if (begin == null)
                return;

            var beginIndex = States.IndexOf(begin);
            if (beginIndex <= 0)
                return;

            var removeCount = States.Count - beginIndex;
            States.RemoveRange(beginIndex, removeCount);
        }

        /// <summary>
        /// Eliminate all states after the current state, 
        /// and add new state after the current state.
        /// </summary>
        /// <param name="newState"></param>
        static void AddNewState(State newState) {
            var eliminateState = CurrentState?.NextState;
            if (eliminateState != null)
                EliminateStates(eliminateState);

            CurrentState.NextState = newState;
            CurrentState.NextCommand = newState.PrevCommand;
            newState.PrevState = CurrentState;
            States.Add(newState);
            CurrentStateIndex = States.IndexOf(newState);
        }

        /// <summary>
        /// Index checker
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        static bool IsInRange(int min, int max, int value) {
            return min <= value && value <= max;
        }

        /// <summary>
        /// To avoid access to an invalid state
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        static int Clamp(int min, int max, int value) {
            if (min > value)
                return min;
            if (max < value)
                return max;

            return value;
        }

        #endregion
    }
    class HistoryTest
    {
        /// <summary>
        /// Undo/Redo example.
        /// </summary>
        /// <param name="args"></param>
        static void Test(string[] args) {
            /// Simple Undo/Redo example.
            Console.WriteLine("Simple undo/redo example.");
            int x = 0;
            Console.WriteLine($"{nameof(x)} is {x}.");

            // Use lambda expression to record your action.
            // Make sure that undo action is correct scheme.
            // If not, it results in an invalid status after you execute "Undo()."

            // Increment x by 1
            History.Record(() => { x++; }, () => { x--; });
            Console.WriteLine($"{nameof(x)} is {x}.");

            // Undo the last action (means decrement x by 1)
            History.Undo();
            Console.WriteLine($"{nameof(x)} is {x}.");

            // Cannot execute undo when you're at the first state.
            History.Undo();
            Console.WriteLine($"{nameof(x)} is {x}.");

            // Redo the action (means increment x by 1)
            History.Redo();
            Console.WriteLine($"{nameof(x)} is {x}.");

            // Cannot execute redo when you at the last state.
            History.Redo();
            Console.WriteLine($"{nameof(x)} is {x}.");


            /// Transaction example
            // Transaction packs multiple actions into one command.

            // Record all commands between "BeginTransaction()" and "EndTransaction()",
            Console.WriteLine();
            Console.WriteLine("Simple transaction command.");
            Console.WriteLine($"{nameof(x)} is {x}.");
            History.BeginTransaction();
            History.Record(() => { x += 10; }, () => { x -= 10; });
            History.Record(() => { x *= 10; }, () => { x /= 10; });
            History.Record(() => { x -= 120; }, () => { x += 120; });
            History.EndTransaction();
            Console.WriteLine($"{nameof(x)} is {x}.");
            History.Undo();
            Console.WriteLine($"{nameof(x)} is {x}.");
            History.Redo();
            Console.WriteLine($"{nameof(x)} is {x}.");

            Console.ReadKey();
        }
    }
}