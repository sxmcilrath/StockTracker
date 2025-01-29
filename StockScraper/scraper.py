import requests
import smtplib
import time
import sys
import numpy as np
from flask import Flask, jsonify, request 
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC

app = Flask(__name__)
@app.route('/get-stock-info', methods=['GET'])  
def get_stock_info():
    symbols = request.args.get('symbols', '') #'' is the default value 
    symbols_list = symbols.split(",")  # Split into list of symbols
    
    #data = np.zeros(len(sys.argv) - 2)
    
    # Ensure arguments
    ''' if len(sys.argv) < 2:
        print("Need args")
        sys.exit(1)
    '''
    # Set Chrome options for headless mode
    chrome_options = Options()
    chrome_options.add_argument("--headless")
    chrome_options.add_argument("--disable-gpu")
    chrome_options.add_argument("--window-size=1920,1080")
    chrome_options.add_argument("--no-sandbox") 
    chrome_options.add_argument("--disable-dev-shm-usage")
    chrome_options.add_argument("--disable-extensions")
    chrome_options.add_argument("--disable-blink-features=AutomationControlled")
    
    # Initialize browser
    driver = webdriver.Chrome(options=chrome_options)

    # NAS driver.get("https://www.nasdaq.com/market-activity/stocks/screener")
    driver.get("https://www.cnbc.com/")

    time.sleep(5)
    
    # Get search box
    driver.execute_script("window.scrollBy(0,1000)") #scroll
    time.sleep(2)
    search_box = driver.find_element(By.CLASS_NAME, "QuoteFinder-searchInput")
    # NAS search_box = driver.find_element(By.CLASS_NAME, "jupiter22-find-symbol__input")

    # Handle each symbol
    #for i in range(1, len(sys.argv)):
    for i in range(0,1):
        #symbol = sys.argv[i]  # Get symbol
        search_box.send_keys(symbols)  # Search for symbol
        search_box.send_keys(Keys.RETURN)  # Press Enter
        time.sleep(5)

        driver.execute_script("window.scrollBy(0,300);")
        
        time.sleep(2)
        # Wait for "Share Volume" to load
        share_volume = "0"
        try:
            WebDriverWait(driver, 10).until(
                EC.presence_of_element_located((By.CLASS_NAME, "QuoteStrip-volume"))
            )
            share_volume = driver.find_element(By.CLASS_NAME, "QuoteStrip-volume").text
            print("Share Volume:", share_volume)
            #data[i-1] = int(share_volume.replace(",", "")) # convert to int and store in data
        except Exception as e:
            print("Element not found:", e)
            return "Error"
    return share_volume

if __name__ == "__main__":
    app.run(debug=True)
