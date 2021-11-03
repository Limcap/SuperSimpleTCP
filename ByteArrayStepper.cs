using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limcap.SSTcp {
	internal class ByteArrayStepper {
		public const int StepMaxSize = ushort.MaxValue;

		public byte[] array { get; private set; }

		internal ByteArrayStepper( byte[] array ) {
			this.array = array;
		}

		public bool NextStep() {
			if (array.Length == 0) {
				return false;
			}
			else if (StepIndex == -1) {
				StepIndex = 0;
				return true;
			}
			else {
				StepIndex += StepMaxSize;
				if (StepIndex >= array.Length) StepIndex = array.Length;
				return StepIndex < array.Length;
			}
		}

		public int StepIndex { get; set; } = -1;

		public int StepSize {
			get => Math.Min( StepMaxSize, array.Length - StepIndex );
		}

		public bool Finished {
			get => StepSize == 0;
		}

		public bool StepIsLast {
			get => StepIndex >= array.Length - StepMaxSize;
		}

		public byte[] Extract() {
			byte[] extracted = new byte[StepSize];
			array.CopyTo( extracted, StepIndex );
			return extracted;
		}


	}
}
