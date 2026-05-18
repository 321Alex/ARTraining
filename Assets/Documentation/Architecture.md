
## Folder Structure

- `Assets/Scripts/AR` contains AR Foundation placement logic.
- `Assets/Scripts/Core` contains lightweight event communication.
- `Assets/Scripts/Interaction` converts touch or mouse input into selectable machine-part events.
- `Assets/Scripts/Machine` owns primitive machine parts, highlights, visual state, and simple animation.
- `Assets/Scripts/Training` owns the guided lesson flow and reusable step data.
- `Assets/Scripts/UI` listens to training events and presents instructions, feedback, and action buttons.

## Architecture

`TrainingManager` is the lesson orchestrator. It listens for events, advances steps, validates the lockout/tagout order, and tells the machine when its safety state changes.

`TrainingEvents` keeps systems decoupled. AR placement, UI buttons, object selection, and the training flow do not need direct references to each other.

`MachinePart` is reusable metadata for any tappable component. A part can be a hazard, a procedure action target, both, or neither.

`HighlightController` owns visual emphasis for a single part. The training flow asks the `MachineController` what to highlight; individual parts know how to display it.

`InstructionPanelUI` is presentation-only. It listens to events and updates text/colors, but does not decide whether a learner is correct.

## Training Flow

1. Place the machine on a detected plane.
2. Tap highlighted hazards: electrical, pressure, moving parts.
3. Complete the procedure in order: power off, release pressure, apply lock, verify safe state.
4. Enter safe maintenance mode.

The machine uses red, yellow, and green to communicate dangerous, partially safe, and safe states.
