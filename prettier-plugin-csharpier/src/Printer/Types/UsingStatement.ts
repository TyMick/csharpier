import { PrintMethod } from "../PrintMethod";
import { SyntaxTreeNode } from "../SyntaxTreeNode";
import { concat, group, hardline, indent, join, softline, line, doubleHardline } from "../Builders";

export interface UsingStatementNode extends SyntaxTreeNode<"UsingStatement"> {

}

export const print: PrintMethod<UsingStatementNode> = (path, options, print) => {
    return "TODO UsingStatement";
};
