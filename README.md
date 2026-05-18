# **ARTraining**

ARTraining is a Unity-based augmented reality training prototype focused on guided industrial safety procedures and instructional interaction design. Learners place a simplified machine into their real environment, inspect highlighted hazards, and complete a guided lockout/tagout procedure before entering a safe maintenance state.

The project was designed to demonstrate practical AR interaction workflows, modular training architecture, event-driven gameplay systems, and reusable instructional content within a lightweight mobile AR experience.

## Project Goals

- Teach the core sequence of a lockout/tagout safety procedure through an interactive AR workflow.
- Replace static instructional content with direct spatial interaction and guided procedural learning.
- Use immediate visual feedback to communicate unsafe, partially safe, and safe machine states.
- Maintain a modular architecture that allows new machine parts, hazards, procedures, and lessons to be added without rewriting core systems.

## Experience

1. The learner scans for a surface and places the training machine onto a detected AR plane.
2. The lesson highlights machine hazards for inspection, including electrical energy, pressure systems, and moving components.
3. The learner completes the lockout/tagout sequence in the correct order:
   - Power off
   - Release pressure
   - Apply lock
   - Verify safe state
4. The machine transitions into safe maintenance mode.

The machine uses red, yellow, and green state feedback to communicate operational safety status at a glance.

## Technical Highlights

- Built in Unity using AR Foundation for mobile augmented reality interaction and guided instructional workflows.
- Supports mobile AR placement and interaction through AR Foundation, including plane detection, touch selection, and world-space instructional UI.
- Uses event-driven communication to decouple AR placement, object selection, UI actions, and lesson progression systems.
- Models selectable machine components with reusable metadata, allowing parts to function as hazards, procedure targets, both, or neither.
- Separates instructional presentation from procedural validation: UI systems present guidance and feedback while the training flow independently validates learner actions and progression.
- Keeps visual emphasis localized through reusable highlight controllers attached to individual machine parts.
- Uses reusable training step data and modular lesson flow systems so additional procedures and machine workflows can be authored without modifying core gameplay systems.
- Includes editor-friendly interaction paths to support rapid iteration without requiring continuous device builds.

## Architecture

TrainingManager acts as the lesson orchestrator. It listens for gameplay events, validates learner actions, advances procedural steps, and updates the machine’s operational safety state.

TrainingEvents provides lightweight event-based communication between systems so AR placement, interaction, UI, and lesson logic remain decoupled.

MachinePart stores reusable metadata for selectable machine components. Parts can represent hazards, procedure interaction targets, informational callouts, or combinations of multiple roles.

HighlightController owns visual emphasis for a single machine component. The training flow requests highlights through the machine controller while each part manages its own presentation behavior.

InstructionPanelUI is presentation-only. It displays procedural instructions, warnings, and feedback without owning lesson validation logic.

## Folder Structure

| Path | Purpose |
| --- | --- |
| `Assets/Scripts/AR` | AR Foundation placement systems and editor-compatible AR support. |
| `Assets/Scripts/Core` | Lightweight event communication and shared utilities. |
| `Assets/Scripts/Interaction` | Touch and mouse-based object interaction systems. |
| `Assets/Scripts/Machine` | Primitive machine components, highlighting, machine state, and visual feedback systems. |
| `Assets/Scripts/Training` | Procedural lesson flow, validation logic, and reusable training step data. |
| `Assets/Scripts/UI` | Instruction panels, warnings, feedback, callouts, and interaction buttons. |
| `Assets/Training` | Reusable ScriptableObject lesson content. |
| `Assets/Scenes` | Unity scenes for the AR training experience. |
| `Assets/Documentation` | Architecture notes and supporting documentation. |
| `Packages` | Unity package manifest and lock file. |
| `ProjectSettings` | Unity project configuration. |
