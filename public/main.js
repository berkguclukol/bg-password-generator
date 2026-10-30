const {app, BrowserWindow, ipcMain, clipboard} = require("electron");
const path = require("path");
const CLIPBOARD_CLEAR_MS = 30000;
let lastCopied = null;

// Clear the clipboard only if it still holds our password, so later copies by the user aren't wiped
const clearClipboardIfOurs = (text) => {
	if (clipboard.readText() === text) clipboard.clear();
};

ipcMain.handle("clipboard:copy", (event, text) => {
	clipboard.writeText(text);
	lastCopied = text;
	setTimeout(() => clearClipboardIfOurs(text), CLIPBOARD_CLEAR_MS);
});

const createWindow = () => {

	const win = new BrowserWindow({
		width: 620,
		height: 525,
		minWidth:620,
		minHeight: 525,
		autoHideMenuBar: true,
		resizable: true,
		webPreferences: {
			nodeIntegration: false,
			contextIsolation: true,
			sandbox: true,
			preload: path.join(__dirname, "preload.js"),
		},
	});
	win.setIcon(path.join(__dirname, "icon.png"));
	win.loadFile(path.join(__dirname, "index.html"));
	win.show();
};

app.whenReady().then(() => {
	createWindow();
	app.on("activate", () => {
		if (BrowserWindow.getAllWindows().length === 0) createWindow();
	});
});
app.on("before-quit", () => {
	if (lastCopied) clearClipboardIfOurs(lastCopied);
});
app.on("window-all-closed", () => {
	if (process.platform !== "darwin") app.quit();
});
