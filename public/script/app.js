let generateBtn = document.getElementById("GenerateBtn");
let generateBtnBottom = document.getElementById("GenerateBtnBottom");
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
range_text.innerHTML = lengthEl.value;

function rangeChange(evtType) {
  lengthEl.addEventListener(evtType, function () {
    window.requestAnimationFrame(function () {
      range_text.innerHTML = lengthEl.value;
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
  copyBtn.innerHTML = `Password Copied!`;
  copyBtn.setAttribute("disabled", "disabled");
  copyBtn.classList.add("disabled");
  setTimeout(() => {
    copyBtn.innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="size-4">
\t\t\t\t<path stroke-linecap="round" stroke-linejoin="round" d="M8.25 7.5V6.108c0-1.135.845-2.098 1.976-2.192.373-.03.748-.057 1.123-.08M15.75 18H18a2.25 2.25 0 0 0 2.25-2.25V6.108c0-1.135-.845-2.098-1.976-2.192a48.424 48.424 0 0 0-1.123-.08M15.75 18.75v-1.875a3.375 3.375 0 0 0-3.375-3.375h-1.5a1.125 1.125 0 0 1-1.125-1.125v-1.5A3.375 3.375 0 0 0 6.375 7.5H5.25m11.9-3.664A2.251 2.251 0 0 0 15 2.25h-1.5a2.251 2.251 0 0 0-2.15 1.586m5.8 0c.065.21.1.433.1.664v.75h-6V4.5c0-.231.035-.454.1-.664M6.75 7.5H4.875c-.621 0-1.125.504-1.125 1.125v12c0 .621.504 1.125 1.125 1.125h9.75c.621 0 1.125-.504 1.125-1.125V16.5a9 9 0 0 0-9-9Z" />
\t\t\t</svg>
\t\t\tCopy Password`;
    copyBtn.removeAttribute("disabled");
    copyBtn.classList.remove("disabled");
  }, 3000);
  return false;
});


generateBtn.addEventListener("click", () => {
  passwordArea.innerText = generatePassword(
      lowercaseEl.checked,
      uppercaseEl.checked,
      numbersEl.checked,
      symbolsEl.checked,
      +lengthEl.value
  );
});
generateBtnBottom.addEventListener("click", () => {
  passwordArea.innerText = generatePassword(
      lowercaseEl.checked,
      uppercaseEl.checked,
      numbersEl.checked,
      symbolsEl.checked,
      +lengthEl.value
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
  return generatedPassword.slice(0, length);
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