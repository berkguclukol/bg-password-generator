const {contextBridge, ipcRenderer} = require("electron");

contextBridge.exposeInMainWorld("clipboardApi", {
	copy: (text) => ipcRenderer.invoke("clipboard:copy", text),
});
