import { Doc } from "prettier";
import { PrintMethod } from "../PrintMethod";
import { HasValue, printValue, SyntaxTreeNode } from "../SyntaxTreeNode";
import { concat, group, hardline, indent, join, softline, line, doubleHardline } from "../Builders";

export interface TryStatementNode extends SyntaxTreeNode<"TryStatement"> {
    tryKeyword: HasValue;
    block: SyntaxTreeNode;
    catches: SyntaxTreeNode[];
    finally?: SyntaxTreeNode;
}

export const print: PrintMethod<TryStatementNode> = (path, options, print) => {
    const parts: Doc[] = [];
    const node = path.getValue();
    parts.push(printValue(node.tryKeyword), path.call(print, "block"), hardline, join(hardline, path.map(print, "catches")));
    if (node.finally) {
        parts.push(hardline, path.call(print, "finally"));
    }
    return concat(parts);
};