#!/usr/bin/env python3

"""
Simple utility for converting one or more PDF files to Markdown using markitdown.

Example:
    python convert_pdfs_to_markdown.py path/to/file.pdf --output-dir exported_md
    python convert_pdfs_to_markdown.py path/to/folder --recursive
"""

from __future__ import annotations

import argparse
import sys
from pathlib import Path
from typing import Iterable, List

from markitdown import MarkItDown


def discover_pdfs(target: Path, recursive: bool) -> List[Path]:
    """Return a list of PDF paths under `target`."""
    suffix = ".pdf"
    if target.is_file():
        return [target] if target.suffix.lower() == suffix else []

    globber: Iterable[Path]
    globber = target.rglob(f"*{suffix}") if recursive else target.glob(f"*{suffix}")
    return [path for path in globber if path.is_file()]


def convert_pdf(pdf_path: Path, output_path: Path, converter: MarkItDown, overwrite: bool) -> None:
    """Convert a single PDF to Markdown."""
    if output_path.exists() and not overwrite:
        raise FileExistsError(f"{output_path} already exists; rerun with --overwrite to replace it.")

    result = converter.convert(str(pdf_path))
    markdown = getattr(result, "text_content", None) or getattr(result, "markdown", "")
    if not markdown:
        raise ValueError(f"No Markdown content returned for {pdf_path}")

    output_path.parent.mkdir(parents=True, exist_ok=True)
    output_path.write_text(markdown, encoding="utf-8")


def main(argv: List[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description="Convert PDF files to Markdown with markitdown.")
    parser.add_argument(
        "source",
        type=Path,
        help="File or directory containing PDFs.",
    )
    parser.add_argument(
        "--output-dir",
        type=Path,
        default=Path("markdown_exports"),
        help="Directory to write Markdown files into (default: ./markdown_exports).",
    )
    parser.add_argument(
        "--recursive",
        action="store_true",
        help="Recursively search directories for PDFs.",
    )
    parser.add_argument(
        "--overwrite",
        action="store_true",
        help="Overwrite existing Markdown files.",
    )

    args = parser.parse_args(argv)
    source: Path = args.source.expanduser().resolve()
    output_dir: Path = args.output_dir.expanduser().resolve()

    if not source.exists():
        parser.error(f"Source path does not exist: {source}")

    pdfs = discover_pdfs(source, args.recursive)
    if not pdfs:
        parser.error("No PDF files found at the specified source.")

    converter = MarkItDown()
    for pdf_path in pdfs:
        relative = pdf_path.name if source.is_file() else pdf_path.relative_to(source)
        output_path = output_dir.joinpath(relative).with_suffix(".md")
        try:
            convert_pdf(pdf_path, output_path, converter, args.overwrite)
            print(f"Converted {pdf_path} -> {output_path}")
        except Exception as exc:  # pylint: disable=broad-except
            print(f"Failed to convert {pdf_path}: {exc}", file=sys.stderr)

    return 0


if __name__ == "__main__":
    raise SystemExit(main())

