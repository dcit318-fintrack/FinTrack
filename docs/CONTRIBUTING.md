# Contributing to FinTrack

Twelve people are working in this repo over two weeks. These rules exist so that nobody's work gets lost and nobody sits blocked waiting on someone else.

**Important:** GitHub branch protection is **not enforced** on our plan. Nothing technically stops you from pushing to `main` or force-pushing over someone's work. These rules hold because we follow them, not because a machine blocks us. Please take them seriously.

---

## The short version

1. Never commit directly to `main`.
2. Never run `git push --force`. Ever.
3. One branch per issue, one pull request per branch.
4. Reference the issue number in your commits.
5. Pull before you start work each day.

---

## Branching

Every piece of work starts from an issue on the board. Create a branch for it:

```bash
git checkout main
git pull origin main
git checkout -b feature/transactions-crud-api
```

**Naming:** `<type>/<area>-<short-description>`

| Type | Use for |
|---|---|
| `feature/` | New functionality |
| `fix/` | Bug fixes |
| `docs/` | Documentation only |
| `chore/` | Tooling, config, cleanup |

Areas: `auth`, `transactions`, `budgets`, `savings`, `reports`, `dashboard`, `db`, `ci`.

Good: `feature/reports-aggregation-endpoints`, `fix/budget-calculation-rounding`, `docs/data-model`
Bad: `mybranch`, `samuel-work`, `update`, `fix2`

Always branch from an up-to-date `main`. Branching from a stale `main` is how you end up with conflicts you didn't cause.

---

## Commits

Write commit messages that explain what changed, in the imperative:

```bash
git commit -m "Add Transactions CRUD endpoints (#18)"
git commit -m "Fix rounding error in budget remaining calculation (#19)"
```

Referencing `#18` links the commit to the issue automatically. When your work fully completes an issue, use `closes` in the **pull request description** (not every commit), so the issue closes on merge:

```
Closes #18
```

Commit often while working. Small commits are easier to review and easier to undo.

---

## Pull requests

When your branch is ready:

```bash
git push origin feature/transactions-crud-api
```

GitHub prints a link — open it, or run `gh pr create`.

**Your PR should include:**
- A title describing the change
- `Closes #N` in the description
- A line on how to test it (which screen, which endpoint, what to expect)
- Screenshots for anything visual

**Keep PRs small.** One screen or one endpoint group, not "all of frontend". A 40-file PR will not get a real review — it'll get an approval nobody means. If your branch is getting large, split it.

**Every PR needs one approval before merging.** Review within your pair for routine work. Anything touching `FinTrack.Shared`, the database layer, or auth goes to Thomas — those are the places where one change breaks three people's work.

Merge your own PR once approved. Delete the branch afterward.

---

## Reviewing

You'll be reviewing your pair partner's work most days. A useful review takes ten minutes:

- Does it do what the issue asked?
- Does it match the API contract in `docs/api-contract.md`?
- Does it build? (CI will tell you.)
- Anything obviously broken, unhandled, or hardcoded?

Approve if it's reasonable. Don't block on style preferences — we don't have time for that and it's not what reviews are for. If something genuinely needs changing, say what and why.

---

## Staying in sync

Start each working session with:

```bash
git checkout main
git pull origin main
```

If you're on a long-running branch and `main` has moved:

```bash
git pull --rebase origin main
```

Rebase rather than merge — it keeps history readable.

**If a push is rejected as non-fast-forward,** it means someone else pushed first. Run `git pull --rebase origin main`, resolve any conflicts, then push again. Do not force-push to get around it.

---

## Conflicts

They will happen. They're normal, not a crisis.

If you hit one you're unsure about, stop and ask in the group chat before resolving it. A badly resolved conflict silently deletes someone's work, and we usually find out days later.

To reduce them: keep PRs small, merge often, and stay in your own area of the codebase. If you need to change something in another pair's area, tell them first.

---

## Project structure

```
src/
  FinTrack.Client/   Blazor WebAssembly — frontend pair
  FinTrack.Server/   ASP.NET Core Web API — backend pair
  FinTrack.Shared/   DTOs used by both — coordinate before changing
tests/
  FinTrack.Tests/    xUnit — QA pair
docs/
  api-contract.md    The agreement between client and server
  data-model.md      Schema and entity relationships
  sprint-plan.md     Day-by-day plan
scripts/
```

**`FinTrack.Shared` is shared for a reason.** Changing a DTO there changes both the client and the server. Never change it silently — raise it in standup first, and update `docs/api-contract.md` in the same PR.

---

## Running it locally

```bash
git clone https://github.com/dcit318-fintrack/FinTrack.git
cd FinTrack
dotnet build
```

Run the API and the client in separate terminals:

```bash
dotnet run --project src/FinTrack.Server
dotnet run --project src/FinTrack.Client
```

Database setup and connection string details are in `docs/data-model.md` — the DB pair maintains that.

Never commit connection strings, JWT signing keys, or passwords. Use `dotnet user-secrets` for local values.

---

## Standups

Daily, 15 minutes. Three things from each pair:

1. What you finished since yesterday
2. What you're doing today
3. What's blocking you

**Blocked for more than half a day? Say so.** Nobody minds being blocked — it's normal on a project with this many dependencies. What hurts is finding out on Day 10 that someone has been stuck since Day 8.

---

## Who owns what

| Area | Pair |
|---|---|
| Project management | Thomas, Lambert |
| UI/UX design | Gerald, Laudina |
| Frontend | Samuel, Ansah |
| Backend | Godsway, Enoch |
| Database & integration | Michael, Akyeampong |
| QA, docs & deployment | Anneta, Matilda |

Issues are labelled by role. If you're unsure whether something is yours, ask in standup rather than assuming.

---

## Questions

Ask in the group chat. A question that takes someone two minutes to answer is always cheaper than half a day of guessing.