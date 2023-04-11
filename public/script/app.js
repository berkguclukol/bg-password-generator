let generateBtn = document.getElementById("GenerateBtn");
let copyBtn = document.getElementById("CopyBtn");
let passwordArea = document.getElementById("password");
let range_text = document.getElementById("range-text");
const lengthEl = document.getElementById("length");
const uppercaseEl = document.getElementById("uppercase");
const lowercaseEl = document.getElementById("lowercase");
const numbersEl = document.getElementById("numbers");
const symbolsEl = document.getElementById("symbols");
rangeChange("mousedown");
rangeChange("mousemove");
rangeChange("keydown");
range_text.innerHTML = "LENGTH: " + lengthEl.value;
function rangeChange(evtType) {
  lengthEl.addEventListener(evtType, function () {
    window.requestAnimationFrame(function () {
      range_text.innerHTML = "LENGTH: " + lengthEl.value;
      lengthEl.setAttribute("aria-valuenow", lengthEl.value);
    });
  });
}
const randomFunc = {
  lower: getRandomLower,
  upper: getRandomUpper,
  number: getRandomNumber,
  symbol: getRandomSymbol,
};
copyBtn.addEventListener("click", () => {
  const textarea = document.createElement("textarea");
  const password = passwordArea.innerText;
  if (!password) {
    return;
  }
  textarea.value = password;
  document.body.appendChild(textarea);
  textarea.select();
  document.execCommand("copy");
  textarea.remove();
  copyBtn.innerText = "COPIED!";
  copyBtn.setAttribute("disabled", "disabled");
  copyBtn.classList.add("disabled");
  setTimeout(() => {
    copyBtn.innerText = "COPY";
    copyBtn.removeAttribute("disabled");
    copyBtn.classList.remove("disabled");
  }, 3000);
  return false;
});

generateBtn.addEventListener("click", () => {
  const length = +lengthEl.value;
  const hasLower = lowercaseEl.checked;
  const hasUpper = uppercaseEl.checked;
  const hasNumber = numbersEl.checked;
  const hasSymbol = symbolsEl.checked;

  passwordArea.innerText = generatePassword(
      hasLower,
      hasUpper,
      hasNumber,
      hasSymbol,
      length
  );
});

function generatePassword(lower, upper, number, symbol, length) {
  let generatedPassword = "";
  const typesCount = lower + upper + number + symbol;
  const typesArr = [{ lower }, { upper }, { number }, { symbol }].filter(
      (item) => Object.values(item)[0]
  );
  if (typesCount === 0) {
    return "";
  }
  for (let i = 0; i < length; i += typesCount) {
    typesArr.forEach((type) => {
      const funcName = Object.keys(type)[0];
      generatedPassword += randomFunc[funcName]();
    });
  }
  const finalPassword = generatedPassword.slice(0, length);
  return finalPassword;
}

function getRandomLower() {
  return String.fromCharCode(Math.floor(Math.random() * 26) + 97);
}

function getRandomUpper() {
  return String.fromCharCode(Math.floor(Math.random() * 26) + 65);
}

function getRandomNumber() {
  return +String.fromCharCode(Math.floor(Math.random() * 10) + 48);
}

function getRandomSymbol() {
  const symbols = "!@#$%^&*(){}[]=<>/,.";
  return symbols[Math.floor(Math.random() * symbols.length)];
}
generateBtn.click();