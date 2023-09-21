package com.jetbrains.rider.roflcopter.settings

import com.jetbrains.rider.settings.simple.SimpleOptionsPage

class MismatchedFileNamesOptionsPage : SimpleOptionsPage(
        name = "Mismatched file names",
        pageId = MismatchedFileNamesOptionsPage::class.simpleName!!
) {

    override fun getId(): String {
        return "preferences." + this.pageId;
    }
}
