#!/usr/bin/env python3
"""Doc drift guard: controllers + policies + critical command coverage."""

from pathlib import Path
import re
import sys


repo = Path(__file__).resolve().parents[1]
controllers_dir = repo / "src" / "TowerOps.Api" / "Controllers"
policies_file = repo / "src" / "TowerOps.Api" / "Authorization" / "ApiAuthorizationPolicies.cs"
imports_dir = repo / "src" / "TowerOps.Application" / "Commands" / "Imports"
reports_dir = repo / "src" / "TowerOps.Application" / "Commands" / "Reports"
api_doc_text = (repo / "docs" / "Api-Doc.md").read_text(encoding="utf-8")
app_doc_text = (repo / "docs" / "Application-Doc.md").read_text(encoding="utf-8")


def contains_token(text: str, token: str) -> bool:
    return re.search(rf"\b{re.escape(token)}\b", text) is not None


def collect_import_commands() -> list[str]:
    commands: set[str] = set()
    for file_path in imports_dir.rglob("*Command.cs"):
        name = file_path.stem
        if name.endswith("Handler") or name.endswith("Validator"):
            continue
        commands.add(name)
    return sorted(commands)


def collect_report_commands() -> list[str]:
    commands = []
    for file_path in reports_dir.glob("*Command.cs"):
        name = file_path.stem
        if name.endswith("Handler") or name.endswith("Validator"):
            continue
        commands.append(name)
    return sorted(set(commands))


def collect_policy_constants() -> list[str]:
    text = policies_file.read_text(encoding="utf-8")
    matches = re.findall(r'public const string (\w+)\s*=\s*"[^"]+";', text)
    return sorted(name for name in matches if name.startswith("Can"))


def fail(title: str, items: list[str]) -> None:
    if not items:
        return
    print(title)
    for item in items:
        print(f"- {item}")
    sys.exit(1)


# 1) Controller coverage in Api-Doc
missing_controllers: list[str] = []
for controller in sorted(controllers_dir.glob("*Controller.cs")):
    name = controller.stem
    if name == "ApiControllerBase":
        continue
    if name not in api_doc_text:
        missing_controllers.append(name)
fail("Missing controllers in docs/Api-Doc.md:", missing_controllers)


# 2) Policy coverage in Api-Doc
missing_policies = [policy for policy in collect_policy_constants() if not contains_token(api_doc_text, policy)]
fail("Missing policies in docs/Api-Doc.md:", missing_policies)


# 3) Critical command coverage in Application-Doc
critical_manual_commands = [
    "LogAuditEntryCommand",
    "CreateApprovalRecordCommand",
    "SubmitForCustomerAcceptanceCommand",
    "AcceptByCustomerCommand",
    "RejectByCustomerCommand",
]

critical_commands = sorted(
    set(critical_manual_commands)
    | set(collect_import_commands())
    | set(collect_report_commands())
)

missing_commands = [command for command in critical_commands if not contains_token(app_doc_text, command)]
fail("Missing critical commands in docs/Application-Doc.md:", missing_commands)

print("Doc drift checks passed: controllers, policies, and critical commands are documented.")
