function findfirstleaf(root) {
    var node = root;
    while (node != null && !node.isLeaf()) {
        if (node.childNodes.length > 0) {
            node = node.getChildAt(0);
        }
        else {
            node = null;
        }
    }
    return node;
}