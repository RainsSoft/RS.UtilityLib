using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RS.UtilityLib.StateMachine.PDN
{
    class TestStateMachine
    {
        public enum UpdatesAction
        {
            Continue,
            Cancel
        }
        public class Control : ISynchronizeInvoke
        {
            public bool InvokeRequired => throw new NotImplementedException();

            public IAsyncResult BeginInvoke(Delegate method, object[] args) {
                throw new NotImplementedException();
            }

            public object EndInvoke(IAsyncResult result) {
                throw new NotImplementedException();
            }

            public object Invoke(Delegate method, object[] args) {
                throw new NotImplementedException();
            }
        }

        public class UpdatesStateMachine : StateMachine
        {
            private Control uiContext;
            public Control UIContext {
                get {
                    return this.uiContext;
                }

                set {
                    this.uiContext = value;
                }
            }

            public UpdatesStateMachine()
                : base(new StartupState(), new object[] { UpdatesAction.Continue, UpdatesAction.Cancel }) {
            }
        }
       
        public abstract class UpdatesState: State
        { 
        }
        public class StartupState : UpdatesState
        {
            public override void ProcessInput(object input, out State newState) {
                if (input.Equals(UpdatesAction.Continue)) {
                    newState = new ReadyToCheckState();
                }
                else if (input.Equals(UpdatesAction.Cancel)) {
                    newState = new CheckingState();
                }
                else {
                    throw new ArgumentException();
                }
            }
        }
        public class CheckingState : UpdatesState
        {
            public override void ProcessInput(object input, out State newState) {
                if (input.Equals(UpdatesAction.Cancel)) {
                    newState = new DoneState();
                }
                else {
                    throw new NotImplementedException();
                }
            }
        }
        public class DoneState : UpdatesState
        {
            public override void OnEnteredState() {
                base.OnEnteredState();
            }

            public override void ProcessInput(object input, out State newState) {
                throw new Exception("The method or operation is not implemented.");
            }

            public DoneState()
                : base() {
            }
        }
        public class ReadyToCheckState: UpdatesState
        {
            public override void OnEnteredState() {
            }

            public override void ProcessInput(object input, out State newState) {
                if (input.Equals(UpdatesAction.Continue)) {
                    newState = new CheckingState();
                }
                else {
                    throw new ArgumentException();
                }
            }

            public ReadyToCheckState()
                : base() {
            }
        }
        public static void test() {
            TestStateMachine tsm = new TestStateMachine();
            tsm.InitUpdates();
            if (!tsm.stateMachineExecutor.IsStarted) {
                //this.stateMachineExecutor.LowPriorityExecution = true;
                tsm.stateMachineExecutor.Start();
                //
                /*
                       if (this.stateMachineExecutor != null)
            {
                if (dr == DialogResult.Yes &&
                    this.stateMachineExecutor.CurrentState is Updates.ReadyToInstallState)
                {
                    this.stateMachineExecutor.ProcessInput(Updates.UpdatesAction.Continue);

                    while (!this.calledFinish)
                    {
                        Application.DoEvents();
                        Thread.Sleep(10);
                    }
                }
            }
                 */
            }

        }
        private StateMachineExecutor stateMachineExecutor;
        private UpdatesStateMachine updatesStateMachine;
        private Control AppWorkspace;

        private void InitUpdates() {
            this.updatesStateMachine = new UpdatesStateMachine();
            this.updatesStateMachine.UIContext = AppWorkspace;

            this.stateMachineExecutor = new StateMachineExecutor(this.updatesStateMachine);
            this.stateMachineExecutor.SyncContext = AppWorkspace;

            this.stateMachineExecutor.StateMachineFinished += OnStateMachineFinished;
            this.stateMachineExecutor.StateBegin += OnStateBegin;
            this.stateMachineExecutor.StateWaitingForInput += OnStateWaitingForInput;
        }

        private void DisposeUpdates() {
            if (this.stateMachineExecutor != null) {
                this.stateMachineExecutor.StateMachineFinished -= OnStateMachineFinished;
                this.stateMachineExecutor.StateBegin -= OnStateBegin;
                this.stateMachineExecutor.StateWaitingForInput -= OnStateWaitingForInput;
                this.stateMachineExecutor.Dispose();
                this.stateMachineExecutor = null;
            }

            this.updatesStateMachine = null;
        }
        private void OnStateBegin(object sender, EventArgs<State> e) {
            throw new NotImplementedException();
        }
        private void OnStateWaitingForInput(object sender, EventArgs<State> e) {
            throw new NotImplementedException();
        }
        private void OnStateMachineFinished(object sender, EventArgs e) {
            throw new NotImplementedException();
        }
    }
}
