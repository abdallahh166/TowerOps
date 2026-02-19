#!/usr/bin/env python3
"""Basic doc drift guard: ensure Api-Doc lists every API controller."""
from pathlib import Path
import sys

repo = Path(__file__).resolve().parents[1]
controllers_dir = repo / "src" / "TelecomPm.Api" / "Controllers"
api_doc = (repo / "docs" / "Api-Doc.md").read_text(encoding="utf-8")

missing = []
for controller in sorted(controllers_dir.glob("*Controller.cs")):
    name = controller.stem
    if name == "ApiControllerBase":
        continue
    if name not in api_doc:
        missing.append(name)

if missing:
    print("Missing controllers in docs/Api-Doc.md:")
    for item in missing:
        print(f"- {item}")
    sys.exit(1)

print("Api-Doc controller coverage check passed.")
