✅ REQUIRED components on every GOAP agent (minimum)
1️⃣ GoapAgent

Role:

Owns the FSM

Handles aborting + replanning

Executes actions


2️⃣ Worker (or a subclass: Builder, Lumberjack, etc.)

Role:

Reports world state

Stores danger + threat memory

Exposes movement (MoveAgent)

Does NOT decide behavior

3️⃣ IGoap implementation

In your design, this is the same component as Worker.

✔ Already satisfied by Worker : IGoap

No extra script needed

4️⃣ NavMeshAgent.CS

Role:

Physical movement

Used by MoveAgent()

✔ Required for any action with requiresInRange() == true

5️⃣ Sight.CS

Role:

Detects enemies

Publishes danger + last known threat position

✔ Required if the agent should react to enemies

6️⃣ Inventory / Backpack / Job-specific data

Role:

Provides resource facts (hasFlour, hasLogs, etc.)

✔ Required if any action references them

Examples: Inventory, Backpack

7️⃣ Action scripts (GoapAction subclasses)

Role:

These are the capabilities of the agent

Planner only considers attached actions

✔ Required — attach ALL applicable actions

Examples:
PickupFlour
BakeBread
DeliverBread
CutTreeAction
HideAction
AttackAction
InvestigateAction
❗ If an action is not attached, the agent literally cannot plan it.

//----------------------------------------------------------------
🧩 OPTIONAL (only if used)
//**********
Animator

If actions or movement drive animation.

//**********
Combat / Health / Morale systems

Only if combat exists.

Examples:

Health
CombatController
Morale

These do not talk to GOAP directly.

//*************

❌ What should NOT be attached

❌ Planner (GoapPlanner)
→ created internally by GoapAgent

❌ FSM
→ internal to GoapAgent

❌ Any “interrupt manager”
→ interrupts are emergent from world state
//------------------------------------------
✅ Minimal example agent (typical)
GameObject: Worker_Agent
├── GoapAgent
├── Builder (Worker)
├── NavMeshAgent
├── Sight
├── Inventory
├── Backpack
├── PickupFlour
├── BakeBread
├── DeliverBread
├── HideAction
├── AttackAction
└── InvestigateAction


That agent will:

gather

bake

deliver

react to danger

investigate threats

hide or fight

replan cleanly

One rule to remember (final)

If a script provides facts or abilities, it’s a component.
If it makes decisions, it lives inside GoapAgent.

If you follow that rule, you’ll never mis-wire an agent again.