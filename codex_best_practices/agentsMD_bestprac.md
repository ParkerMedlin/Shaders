Custom instructions with AGENTS.md
Give Codex extra instructions and context for your project

Codex reads AGENTS.md files before doing any work. By layering global guidance with project-specific overrides, you can start each task with consistent expectations, no matter which repository you open.

How Codex discovers guidance

Codex builds an instruction chain when it starts (once per run; in the TUI this usually means once per launched session). Discovery follows this precedence order:

Global scope: In your Codex home directory (defaults to ~/.codex, unless you set CODEX_HOME), Codex reads AGENTS.override.md if it exists. Otherwise, Codex reads AGENTS.md. Codex uses only the first non-empty file at this level.
Project scope: Starting at the project root (typically the Git root), Codex walks down to your current working directory. If Codex cannot find a project root, it only checks the current directory. In each directory along the path, it checks for AGENTS.override.md, then AGENTS.md, then any fallback names in project_doc_fallback_filenames. Codex includes at most one file per directory.
Merge order: Codex concatenates files from the root down, joining them with blank lines. Files closer to your current directory override earlier guidance because they appear later in the combined prompt.
Codex skips empty files and stops adding files once the combined size reaches the limit defined by project_doc_max_bytes (32 KiB by default). For details on these knobs, see Project instructions discovery. Raise the limit or split instructions across nested directories when you hit the cap.

Create global guidance

Create persistent defaults in your Codex home directory so every repository inherits your working agreements.

Ensure the directory exists:

mkdir -p ~/.codex

Create ~/.codex/AGENTS.md with reusable preferences:

# ~/.codex/AGENTS.md

## Working agreements

- Always run `npm test` after modifying JavaScript files.
- Prefer `pnpm` when installing dependencies.
- Ask for confirmation before adding new production dependencies.

Run Codex anywhere to confirm it loads the file:

codex --ask-for-approval never "Summarize the current instructions."

Expected: Codex quotes the items from ~/.codex/AGENTS.md before proposing work.

Use ~/.codex/AGENTS.override.md when you need a temporary global override without deleting the base file. Remove the override to restore the shared guidance.

Layer project instructions

Repository-level files keep Codex aware of project norms while still inheriting your global defaults.

In your repository root, add an AGENTS.md that covers basic setup:

# AGENTS.md

## Repository expectations

- Run `npm run lint` before opening a pull request.
- Document public utilities in `docs/` when you change behavior.

Add overrides in nested directories when specific teams need different rules. For example, inside services/payments/ create AGENTS.override.md:

# services/payments/AGENTS.override.md

## Payments service rules

- Use `make test-payments` instead of `npm test`.
- Never rotate API keys without notifying the security channel.

Start Codex from the payments directory:

codex --cd services/payments --ask-for-approval never "List the instruction sources you loaded."

Expected: Codex reports the global file first, the repository root AGENTS.md second, and the payments override last.

Codex stops searching once it reaches your current directory, so place overrides as close to specialized work as possible.

Here is a sample repository after you add a global file and a payments-specific override:

AGENTS.md
Repository expectations
services/
payments/
AGENTS.md
Ignored because an override exists
AGENTS.override.md
Payments service rules
README.md
search/
Customize fallback filenames

If your repository already uses a different filename (for example TEAM_GUIDE.md), add it to the fallback list so Codex treats it like an instructions file.

Edit your Codex configuration:

# ~/.codex/config.toml
project_doc_fallback_filenames = ["TEAM_GUIDE.md", ".agents.md"]
project_doc_max_bytes = 65536

Restart Codex or run a new command so the updated configuration loads.

Now Codex checks each directory in this order: AGENTS.override.md, AGENTS.md, TEAM_GUIDE.md, .agents.md. Filenames not on this list are ignored for instruction discovery. The larger byte limit allows more combined guidance before truncation.

With the fallback list in place, Codex treats the alternate files as instructions:

TEAM_GUIDE.md
Detected via fallback list
.agents.md
Fallback file in root
support/
AGENTS.override.md
Overrides fallback guidance
playbooks/
Set the CODEX_HOME environment variable when you want a different profile, such as a project-specific automation user:

CODEX_HOME=$(pwd)/.codex codex exec "List active instruction sources"

Expected: The output lists files relative to the custom .codex directory.