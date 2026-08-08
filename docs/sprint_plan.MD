# FinTrack sprint plan

**Duration:** 14 days
**Owner:** PMs (Thomas, Lambert)
**Board:** github.com/dcit318-fintrack/FinTrack/issues

This plan orders the 43 issues by dependency, not by role. The rule underneath it: nothing that blocks another pair gets scheduled late, and nothing waits for something it doesn't actually need.

Issue numbers in brackets. `→` means "blocks".

---

## The three things that gate everything

| Issue | Owner | Due | Blocks |
|---|---|---|---|
| #4 API contract doc | PM + Backend | Day 1 | Frontend and Backend parallel work |
| #31 Database schema | Database | Day 2 | Backend entity models, all CRUD |
| #13 Hi-fi mockups | UI/UX | Day 5 | Frontend screens |

If any of these slip, the sprint slips. Everything else has slack.

---

## Days 1–2 — foundation

Nobody is blocked yet, so everyone starts something.

**PM**
- #2 Write `CONTRIBUTING.md` — branching rules, PR expectations, no force-push
- #3 Create the project board, add all 43 issues
- #4 Finalise the API contract with Backend *(draft already committed)*
- #5 Standups begin Day 1 and run to Day 14

**UI/UX**
- #10 Wireframe all 4 screens — start immediately, this is the long pole
- #11 Define the design system in parallel

**Database**
- #31 Design schema: Users, Transactions, Budgets, SavingsGoals, Categories

**Backend**
- #16 Scaffold API structure — controllers, services, DTO folders
- #20 Work through the API contract with Frontend, settle the open questions

**Frontend**
- #24 Scaffold Blazor routing, shared layout, nav — needs no mockups

**QA**
- #37 Write the test plan from the spec
- #38 Set up the CI workflow (build + test on PR)

**End of Day 2 checkpoint:** contract signed off, schema agreed, wireframes reviewed. If any of these isn't done, say so in standup — don't let it drift into Day 3.

---

## Days 3–5 — parallel build

The point of Days 1–2 was to make this possible: three pairs building at once with nobody waiting.

**Database**
- #32 EF Core DbContext, initial migration, seed data
- #33 Finalise entity models against the agreed DTOs *(with Backend)*

**Backend**
- #17 Identity + JWT — register, login, refresh
- #18 Transactions CRUD

**Frontend**
- #25 Dashboard, built against mocked data matching the contract
- #26 Transactions screen, same approach

**UI/UX**
- #12 Reports chart layouts
- #13 Hi-fi mockups delivered by Day 5 — hard deadline

**QA**
- #39 Start the documentation skeleton, fill in as features land

**End of Day 5 checkpoint:** mockups delivered, auth working end to end, seeded database running locally for everyone.

---

## Days 6–7 — first integration

The first real handoff. Expect this to take longer than planned; it always does.

**Backend**
- #19 Budgets and Savings Goals CRUD

**Frontend**
- #27 Login/register UI wired to the live JWT endpoints — the first mocked-to-real swap
- Rework Dashboard and Transactions against the real mockups

**Database**
- Support #17–#19 as data issues surface

**PM**
- #6 Confirm wireframes are locked and Frontend is unblocked
- Watch the auth hookup closely — if the token flow doesn't work, nothing downstream does

**End of Week 1:** a user can register, log in, and add a transaction that persists. If that works, you're on schedule.

---

## Days 8–10 — the Reports chain

The one place with a hard sequence. Three handoffs stacked, and the last one is on the critical path to the demo.

```
#34 DB query support  →  #21 Backend aggregations  →  #29 Frontend charts
```

**Database**
- #34 Optimise the aggregation queries — start Day 8, don't wait
- #35 Data integrity: cascading deletes, budget-vs-transaction consistency

**Backend**
- #21 Reports endpoints — spending by category, income vs expense
- #22 Validation and consistent error responses across all endpoints

**Frontend**
- #28 Budget & Savings screen — independent of the Reports chain, do it while waiting

**QA**
- #40 Begin testing whatever is finished; don't wait for a complete build

**Risk note:** if #21 isn't done by end of Day 10, Frontend cannot start #29 with enough time. That's the single most likely point of failure in this sprint. PM watches it daily.

---

## Days 11–12 — full integration

**Frontend**
- #29 Reports screen with charts
- #30 Replace all remaining mocks with live API calls, add loading and error states

**Backend**
- #23 Fix bugs surfaced during hookup — this is the whole job now

**Database**
- #36 DB-side debugging support

**UI/UX**
- #14 Responsive, empty, error, and loading states — hand specs to Frontend
- #15 Usability pass on the working build

**PM**
- #7 Track integration daily
- #8 **Scope decision point, end of Day 12.** Anything not working by now gets cut unless it's one of the three core functionalities. Make the call and record it in the issue thread.

---

## Day 13 — QA and deploy

**QA**
- #40 Full manual pass across all four screens
- #41 Deploy the API and the WASM client

**Everyone else** — fix bugs QA logs. No new features from Day 13 onward. That's a hard line; new code on Day 13 is how demos break.

---

## Day 14 — polish and rehearse

**QA**
- #42 Finalise README, setup instructions, submission docs
- #43 Support the demo rehearsal

**PM**
- #9 Demo script and a full rehearsal against the deployed build

**Everyone** — one rehearsal, all twelve present, on the deployed build rather than localhost.

---

## Slack and buffers

Days 6–7 and 11–12 have deliberate slack. If Reports slips, borrow from Days 11–12. If the schema slips, you're in trouble immediately — that's why it's a Day-2 deadline rather than a Day-3 one.

Nothing here has a buffer after Day 13. If you're behind by then, cut scope rather than pushing the deadline.

---

## What gets cut first if you run out of time

In order:

1. Savings goals — budgets alone satisfy "budgets/savings"
2. Report granularity options — ship monthly only
3. Pagination on transactions — a flat list is fine at demo data volumes
4. Transaction editing — add and delete cover the demo

**Never cut:** auth, transaction CRUD, budgets, and at least one working chart. Those are the three core functionalities the project is graded on.

---

## PM ownership split

**Thomas** — repo, #2, #3, #4, #7, PR reviews on `FinTrack.Shared` and the data layer, technical tiebreaks.

**Lambert** — board upkeep, assigning issues as invites land, #5, #6, #8, #9, standup facilitation, lecturer comms, submission logistics.

---

## Standing rules

- Branch per issue: `feature/<area>-<description>`. Never commit to `main`.
- Reference the issue in the commit: `git commit -m "Add budget endpoints (closes #19)"`.
- Blocked for more than half a day? Say it in standup. Silent blockers are what kill two-week sprints.
- Branch protection is not enforced on our GitHub plan. These rules hold because people follow them, not because a machine stops them.