const {app, BrowserWindow, ipcMain} = require("electron");
const path = require("path");
const ipc = ipcMain;
const createWindow = () => {
	const win = new BrowserWindow({
		width: 450,
		height: 650,
		//autoHideMenuBar: true,
		webPreferences: {
			nodeIntegration: true,
			contextIsolation: false,
			preload: path.join(__dirname, "preload.js"),
		},
	});
	win.setIcon(path.join(__dirname, "icon.png"));
	win.loadFile(path.join(__dirname, "public/index.html"));
	win.show();
    ipc.on("app/minimize", () => {
		win.minimize();
	});
	ipc.on("app/maximize", () => {
		//
	});
	ipc.on("app/quit", () => {
		app.quit();
	});
};

app.whenReady().then(() => {
	createWindow();
	app.on("activate", () => {
		if (BrowserWindow.getAllWindows().length === 0) createWindow();
	});
});
app.on("window-all-closed", () => {
	if (process.platform !== "darwin") app.quit();
});
