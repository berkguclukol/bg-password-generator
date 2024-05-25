const {app, BrowserWindow, ipcMain} = require("electron");
const path = require("path");
const ipc = ipcMain;
const createWindow = () => {
	const splash = new BrowserWindow({
		width: 500,
		height: 300,
		transparent: true,
		frame: false,
		alwaysOnTop: true,
	});
	splash.setIcon(path.join(__dirname, "icon.png"));
	splash.loadFile(path.join(__dirname, "public/splash.html"));
	splash.center();
	const win = new BrowserWindow({
		width: 520,
		height: 650,
		maximizable: false,
		autoHideMenuBar: true,
		resizable: true,
		frame: false,
		show: false,
		webPreferences: {
			nodeIntegration: true,
			contextIsolation: false,
			preload: path.join(__dirname, "preload.js"),
		},
	});
	win.setIcon(path.join(__dirname, "icon.png"));
	win.loadFile(path.join(__dirname, "public/index.html"));
	setTimeout(function () {
		splash.close();
		win.show();
	}, 5000);
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
