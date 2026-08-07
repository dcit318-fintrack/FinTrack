# create-issues.ps1
# Bulk-creates FinTrack issues from the team requirement spec.
# Run from inside the repo folder, after `gh auth login`.
#
#   .\create-issues.ps1

$issues = @(

  # ---------------- PROJECT MANAGERS ----------------
  @{ t = "Break the 4 screens into tickets and assign to devs"
     b = "Split Dashboard, Transactions, Budget & Savings, and Reports into granular tickets. Assign each to the relevant role pair once org invites are accepted."
     l = "role: pm"; m = "Week 1" }

  @{ t = "Document branching strategy and PR review rules"
     b = "Write CONTRIBUTING.md covering: branch naming (feature/<area>-<description>), PR size expectations, who reviews what, and the no-force-push rule. Note that branch protection is not enforced on our plan, so this is convention."
     l = "role: pm"; m = "Week 1" }

  @{ t = "Set up project board"
     b = "Create a GitHub Projects board with columns: Backlog, In Progress, In Review, Done. Add all issues and set up automation where possible."
     l = "role: pm"; m = "Week 1" }

  @{ t = "Create shared API contract doc"
     b = "Seed docs/api-contract.md with endpoint paths, request/response DTO shapes, and status codes for all 4 feature areas. Backend and Frontend fill it in together so both can work in parallel."
     l = "role: pm"; m = "Week 1" }

  @{ t = "Schedule and run daily 15-min standups"
     b = "Set a fixed time. Each pair reports progress, blockers, next steps. Log blockers publicly, especially Backend/DB to Frontend integration points."
     l = "role: pm"; m = "Week 1" }

  @{ t = "Lock wireframes with UI/UX by end of Week 1"
     b = "Coordinate with the UI/UX pair so hi-fi mockups land by Day 4-5. Frontend is blocked until this is done."
     l = "role: pm"; m = "Week 1" }

  @{ t = "Track frontend/API integration testing"
     b = "Monitor the hookup between Blazor client and the live API. Escalate and unblock failures fast; this is where the schedule usually slips."
     l = "role: pm"; m = "Week 2" }

  @{ t = "Scope management: cut non-essential features if needed"
     b = "Protect the 3 core functionalities (transactions, budgets/savings, reports). Anything outside those gets cut first if time runs short."
     l = "role: pm"; m = "Week 2" }

  @{ t = "Prepare final demo and confirm release readiness"
     b = "Write the demo script, run a rehearsal, and confirm with QA that the deployed build is demo-ready."
     l = "role: pm"; m = "Week 2" }

  # ---------------- UI/UX ----------------
  @{ t = "Wireframe all 4 screens"
     b = "Low-fidelity wireframes for Dashboard, Transactions (add/edit/delete), Budget & Savings, and Reports. Front-loaded work: everyone else depends on this."
     l = "role: uiux"; m = "Week 1" }

  @{ t = "Define the design system"
     b = "Colors, typography, spacing scale, and component states for buttons, forms, cards, and the add/edit transaction modal."
     l = "role: uiux"; m = "Week 1" }

  @{ t = "Mock the Reports charts layout"
     b = "Layout for spending-by-category and income-vs-expense-over-time charts, so Frontend knows what to build with Chart.js."
     l = "role: uiux"; m = "Week 1" }

  @{ t = "Deliver hi-fi mockups by Day 4-5"
     b = "High-fidelity mockups in Figma or similar, covering all 4 screens. Hard deadline: Frontend cannot start real UI work until these land."
     l = "role: uiux"; m = "Week 1" }

  @{ t = "Specify responsive, empty, error, and loading states"
     b = "Define what each screen looks like with no data, while loading, and on failure. Hand these to Frontend for implementation."
     l = "role: uiux"; m = "Week 2" }

  @{ t = "Usability pass on the working build"
     b = "Walk the real build as a user would. Flag friction points and log them as issues before demo rehearsal."
     l = "role: uiux"; m = "Week 2" }

  # ---------------- BACKEND ----------------
  @{ t = "Scaffold Web API structure (controllers, services, DTOs)"
     b = "Set up the folder and layer structure in FinTrack.Server. Shared DTOs go in FinTrack.Shared so the Blazor client references the same types."
     l = "role: backend"; m = "Week 1" }

  @{ t = "Implement ASP.NET Core Identity + JWT auth"
     b = "Register, login, and token refresh endpoints. Configure JWT issuance and validation."
     l = "role: backend"; m = "Week 1" }

  @{ t = "Build Transactions CRUD endpoints"
     b = "Create, read, update, delete for transactions, scoped to the authenticated user."
     l = "role: backend"; m = "Week 1" }

  @{ t = "Build Budgets and Savings Goals CRUD endpoints"
     b = "Create, read, update, delete for category budgets and savings goals, scoped to the authenticated user."
     l = "role: backend"; m = "Week 1" }

  @{ t = "Agree request/response DTO shapes with Frontend"
     b = "Settle DTO shapes early and record them in docs/api-contract.md. This is what unblocks Blazor work before the API is finished."
     l = "role: backend"; m = "Week 1" }

  @{ t = "Build Reports aggregation endpoints"
     b = "Spending by category and income vs. expense over time. Coordinate with the DB pair on query efficiency."
     l = "role: backend"; m = "Week 2" }

  @{ t = "Add validation, error handling, and consistent response format"
     b = "Model validation on all inputs, a uniform error response shape, and correct status codes throughout."
     l = "role: backend"; m = "Week 2" }

  @{ t = "Support integration testing and fix hookup bugs"
     b = "Pair with Frontend during API hookup; fix defects surfaced during integration."
     l = "role: backend"; m = "Week 2" }

  # ---------------- FRONTEND ----------------
  @{ t = "Scaffold Blazor WASM routing and shared layout"
     b = "Routes for all 4 screens plus shared layout and navigation in FinTrack.Client."
     l = "role: frontend"; m = "Week 1" }

  @{ t = "Build Dashboard screen"
     b = "Total income, total expenses, current balance, and recent transactions. Build against mocked data until the API is live."
     l = "role: frontend"; m = "Week 1" }

  @{ t = "Build Transactions screen"
     b = "List view plus add, edit, and delete forms. Build against mocked data until the API is live."
     l = "role: frontend"; m = "Week 1" }

  @{ t = "Build login and register UI wired to JWT"
     b = "Auth forms plus token storage and attaching the bearer token to API calls. Hook up once Backend exposes the endpoints."
     l = "role: frontend"; m = "Week 1" }

  @{ t = "Build Budget & Savings screen"
     b = "Set per-category monthly limits and track progress toward savings targets."
     l = "role: frontend"; m = "Week 2" }

  @{ t = "Build Reports screen with charts"
     b = "Chart.js or Blazor charting components for spending by category and income vs. expense over time, once aggregation endpoints are ready."
     l = "role: frontend"; m = "Week 2" }

  @{ t = "Replace mocked data with live API calls"
     b = "Swap all mocks for real HTTP calls. Implement loading and error states per the UI/UX specs."
     l = "role: frontend"; m = "Week 2" }

  # ---------------- DATABASE & INTEGRATION ----------------
  @{ t = "Design database schema"
     b = "Users, Transactions, Budgets, SavingsGoals, Categories, with relationships and constraints. Document in docs/data-model.md."
     l = "role: database"; m = "Week 1" }

  @{ t = "Set up EF Core DbContext, migrations, and seed data"
     b = "DbContext configuration, initial migration, and seed data so everyone has a working local dev database."
     l = "role: database"; m = "Week 1" }

  @{ t = "Finalize entity models against API DTOs"
     b = "Work with Backend so entity models and the agreed DTO shapes line up without awkward mapping."
     l = "role: database"; m = "Week 1" }

  @{ t = "Optimize queries for Reports aggregations"
     b = "Make sure spending-by-category and trend-over-time queries stay fast as transaction volume grows."
     l = "role: database"; m = "Week 2" }

  @{ t = "Handle data integrity rules"
     b = "Cascading deletes and budget-vs-transaction consistency checks."
     l = "role: database"; m = "Week 2" }

  @{ t = "Support integration debugging (DB side)"
     b = "Be available during Frontend/Backend hookup to diagnose data-layer issues."
     l = "role: database"; m = "Week 2" }

  # ---------------- QA, DOCS & DEPLOYMENT ----------------
  @{ t = "Write test plan for core functionality and auth"
     b = "Cover transactions, budgets/savings, reports, and the auth flow. Write cases as features are defined, not after they ship."
     l = "role: qa-docs"; m = "Week 1" }

  @{ t = "Set up CI build checks on PRs"
     b = "GitHub Actions workflow running dotnet build (and dotnet test) on every pull request."
     l = "role: qa-docs"; m = "Week 1" }

  @{ t = "Start drafting user and technical documentation"
     b = "Begin the docs skeleton in Week 1 and fill it in as features land, rather than writing everything at the end."
     l = "role: qa-docs"; m = "Week 1" }

  @{ t = "Execute manual test cases across all 4 screens"
     b = "Run the test plan against the working build. Log and triage bugs with the relevant dev pair."
     l = "role: qa-docs"; m = "Week 2" }

  @{ t = "Prepare deployment for API and Blazor client"
     b = "Host the Web API and the WASM client (Azure App Service or similar). Document the deployment steps."
     l = "role: qa-docs"; m = "Week 2" }

  @{ t = "Finalize README, setup instructions, and submission docs"
     b = "Complete project documentation for submission, including how to clone, configure, and run locally."
     l = "role: qa-docs"; m = "Week 2" }

  @{ t = "Support final demo rehearsal"
     b = "Verify the deployed build works end to end during rehearsal and confirm release readiness with the PMs."
     l = "role: qa-docs"; m = "Week 2" }
)

$count = 0
foreach ($i in $issues) {
    $count++
    Write-Host "[$count/$($issues.Count)] $($i.t)" -ForegroundColor Cyan
    gh issue create --title $i.t --body $i.b --label $i.l --milestone $i.m
    Start-Sleep -Milliseconds 400   # stay under GitHub's content-creation rate limit
}

Write-Host "`nDone - created $count issues." -ForegroundColor Green
