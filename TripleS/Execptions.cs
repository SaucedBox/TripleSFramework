using System;

namespace TripleS {

    public class SequenceAlreadyCheckedExecption : Exception {
        public SequenceAlreadyCheckedExecption()
        {
        }

        public SequenceAlreadyCheckedExecption(string message)
            : base(message)
        {
        }
    }

    public class LayerLengthExecption : Exception {
        public LayerLengthExecption()
        {
        }

        public LayerLengthExecption(string message)
            : base(message)
        {
        }
    }

    public class ParamExecption : Exception {
        public ParamExecption()
        {
        }

        public ParamExecption(string message)
            : base(message)
        {
        }
    }

    public class GameStateExecption : Exception {
        public GameStateExecption()
        {
        }

        public GameStateExecption(string message)
            : base(message)
        {
        }
    }

    public class ScriptExecption : Exception {
        public ScriptExecption()
        {
        }

        public ScriptExecption(string message)
            : base(message)
        {
        }
    }
}
